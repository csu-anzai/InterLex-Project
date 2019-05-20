namespace Interlex.Crawler.Crawlers.FR
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;
    using Interlex.Crawler.Model;
    using log4net;

    /// <summary>
    /// Represents crawler for the legi france open data portal for the database CAPP, CASS, INCA, JADE
    /// </summary>
    public class LegiFrance : BaseGenericCrawler
    {
        private static readonly IReadOnlyDictionary<String, String> ftps = new Dictionary<string, string>
        {
            ["capp_juri_"] = "ftp://echanges.dila.gouv.fr/CAPP/",
            ["cass_juri_"] = "ftp://echanges.dila.gouv.fr/CASS/",
            ["inca_juri_"] = "ftp://echanges.dila.gouv.fr/INCA/",
            ["jade_jade_"] = "ftp://echanges.dila.gouv.fr/JADE/"
        };

        public LegiFrance(ILog logger) : base(logger)
        {
        }

        public override Task StartAsync()
        {
            foreach (var (prefix, url) in ftps)
            {
                // var afterDate = DateTime.MinValue;
                this.SyncWithLatestUpdates_NewFTP(url,  prefix);
            }

            return Task.CompletedTask;
        }

        private void SyncWithLatestUpdates_NewFTP(string url,  string prefix)
        {
            var ftpEntriesInfo = this.GetTarGzFilesDetail(url)
                .Where(x => x.FileName.Contains("freemium", StringComparison.OrdinalIgnoreCase) == false)
                .OrderBy(x => x.FileDate)
                .ToList();

            var latestWithoutDublicate = ftpEntriesInfo
                .SelectMany(f =>
                {
                    var unzipedFiles = this.DownloadFiles(f);

                    this.Logger.Info(f.FileName);

                    return unzipedFiles;

                })
                .GroupBy(x => x.filePath)
                .Select(g => g.OrderByDescending(x => x.date).First())
                .Select(x => (x.filePath, x.data))
                .ToArray();

            this.PushToDatabase(latestWithoutDublicate, prefix);
        }

        private IEnumerable<FTPEntity> GetTarGzFilesDetail(String ftpUrl)
        {
            List<FTPEntity> result = new List<FTPEntity>();

            try
            {
                var ftpRequester = WebRequest.Create(ftpUrl) as FtpWebRequest;
                if (ftpRequester != null)
                {
                    ftpRequester.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    using (var resonse = ftpRequester.GetResponse())
                    {
                        var responseStream = resonse.GetResponseStream();
                        if (responseStream != null)
                        {
                            using (var streamReader = new StreamReader(responseStream))
                            {
                                var detailAggregatge = streamReader.ReadToEnd();
                                var detailLines = detailAggregatge.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                                var filesDetailLines = detailLines.Where(x => x.StartsWith("-") && x.EndsWith(".tar.gz"));

                                foreach (var detailLine in filesDetailLines)
                                {
                                    /*
                                        -rw-rw-r--    1 919      501      240045532 Dec 04 07:35 Apis_capp_global_20151204-082045.tar.gz
                                        -rw-rw-r--    1 919      501      211800045 Dec 03 14:39 Apis_cass_global_20151203-153959.tar.gz
                                    */

                                    String[] majorParts = detailLine.Split(new String[] { "      ", "    " }, StringSplitOptions.RemoveEmptyEntries);

                                    // reprets info like size, date and name
                                    String mainDesription = majorParts.Last();

                                    String[] majorSubParts = mainDesription.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                                    String fileName = majorSubParts[4]; // apis_20151214-211526.tar.gz
                                    String unformatedDate = fileName.Split('_').Last().Split('-').First();
                                    DateTime date = DateTime.ParseExact(unformatedDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                                    String size = majorSubParts[0];

                                    FTPEntity ftpEntity = new FTPEntity()
                                    {
                                        FileDate = date,
                                        FileName = fileName,
                                        FileSize = size,
                                        FilePath = Path.Combine(ftpUrl, fileName)
                                    };

                                    if (ftpEntity.FileName.EndsWith(".tar.gz"))
                                    {
                                        result.Add(ftpEntity);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Logger.Error(ftpUrl, e);
            }

            return result;
        }

        private IEnumerable<(String filePath, DateTime date, Byte[] data)> DownloadFiles(FTPEntity ftpEntity)
        {
            try
            {
                var request = (FtpWebRequest)WebRequest.Create(ftpEntity.FilePath);
                request.Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
                request.ReadWriteTimeout = request.Timeout;

                using (var webResponse = (FtpWebResponse)request.GetResponse())
                using (var responseStream = webResponse.GetResponseStream())
                {
                    var unziped = UnzipToMemory(originalZipStream: responseStream);

                    return unziped.Select(x => (x.Key, ftpEntity.FileDate, x.Value));
                }
            }
            catch (Exception e)
            {
                this.Logger.Error(ftpEntity.FilePath, e);

                return Enumerable.Empty<(String, DateTime, byte[])>();
            }
        }

        private static Dictionary<String, Byte[]> UnzipToMemory(Stream originalZipStream)
        {

            var result = new Dictionary<string, Byte[]>();

            using (var stream = new MemoryStream())
            {
                Stream gzipStream = new GZipInputStream(originalZipStream);
                TarInputStream tarStream = new TarInputStream(gzipStream);

                TarEntry tarEntry;
                while ((tarEntry = tarStream.GetNextEntry()) != null)
                {
                    if (tarEntry.IsDirectory)
                    {
                        continue;
                    }

                    string name = tarEntry.Name.Replace('/', Path.DirectorySeparatorChar);
                    if (!name.EndsWith(".xml"))
                    {
                        continue;
                    }

                    if (Path.IsPathRooted(name))
                    {
                        name = name.Substring(Path.GetPathRoot(name).Length);
                    }

                    using (var contentStream = new MemoryStream())
                    {
                        tarStream.CopyEntryContents(contentStream);
                        result.Add(name.ToLower(), contentStream.ToArray());
                    }
                }

                tarStream.Close();
            }

            return result;
        }

        private void PushToDatabase(IEnumerable<(String filePath, Byte[] data)> downloads, string prefix)
        {
            foreach (var (filePath, data) in downloads)
            {
                try
                {
                    var fileName = $"{prefix}{Path.GetFileName(filePath)}";
                    var documentGroup = new DocumentGroupModel
                    {
                        CrawlerId = this.CrawlerId,
                        Name = Path.GetFileNameWithoutExtension(fileName),
                        TwoLetterLanguage = "FR",
                        Documents =
                        {
                            new DocumentModel
                            {
                                Raw = data,
                                Name = fileName,
                                Format = "text/xml",
                                Url = fileName,
                            }
                        }
                    };

                    this.DocumentGroupManager.AddOrUpdateDocumentGroup(documentGroup);
                }
                catch (Exception e)
                {
                    this.Logger.Error(filePath, e);
                }
            }
        }

        [DebuggerDisplay("{FileName} - {FileDate} - {FilePath}")]
        private class FTPEntity
        {
            public String FilePath { get; set; }
            public String FileName { get; set; }
            public String FileSize { get; set; }
            public DateTime FileDate { get; set; }
        }
    }
}
