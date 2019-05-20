namespace Interlex.Crawler.Crawlers.DE
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using Interlex.Crawler.Model;
    using log4net;

    public abstract class BaseBundes : BaseGenericCrawler
    {
        private const string MainRechtsprechungUrl = "https://www.rechtsprechung-im-internet.de/jportal/portal/t/64j/page/bsjrsprod.psml/js_peid/Suchportlet1/media-type/html?";
        private const string BaseRechtsprechungUrl = "https://www.rechtsprechung-im-internet.de/jportal/portal/t/64u/page/bsjrsprod.psml?doc.hl=1";
        private const string Rechtsprechung = "https://www.rechtsprechung-im-internet.de";

        public BaseBundes(ILog logger) : base(logger)
        {
        }

        public abstract override Task StartAsync();

        public async Task LoadAsync(string baseUrl, string mainUrl, int fromYear, int toYear, string crawlerName)
        {
            this.Logger.Info($"Start {crawlerName} - {DateTime.UtcNow}");

            for (int year = fromYear; year <= toYear; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    var pageUrl = $"{baseUrl}{year}-{month}";

                    try
                    {
                        var isNextPage = true;
                        var pageNumber = 0;
                        var page = await this.Http.GetAsync(pageUrl);

                        do
                        {
                            var htmlDocument = page.GetAsHtml();
                            await this.ParseDocumentsAsync(page, mainUrl);
                            isNextPage = this.CheckIsNextpage(htmlDocument);

                            if (isNextPage)
                            {
                                pageNumber++;
                                var nextPageUrl = $"{baseUrl}{year}-{month}&Seite={pageNumber}";

                                try
                                {
                                    page = await this.Http.GetAsync(nextPageUrl);
                                }
                                catch (Exception ex)
                                {
                                    this.Logger.Error($"Download year Url: {nextPageUrl}", ex);
                                }
                            }

                        } while (isNextPage);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error($"Download year Url: {pageUrl}", ex);
                    }
                }
            }

            this.Logger.Info($"End {crawlerName} - {DateTime.UtcNow}");
        }

        private async Task ParseDocumentsAsync(HttpModel page, string mainUrl)
        {
            var htmlDocument = page.GetAsHtml();
            var nodes = htmlDocument.DocumentNode.SelectNodes("//form[@name='list']/table/tbody/tr");

            if (nodes?.Count > 0)
            {
                foreach (var node in nodes)
                {
                    var href = this.ExtractHrefLink(node.OuterHtml);
                    if (!string.IsNullOrEmpty(href))
                    {
                        var url = $"{mainUrl}{WebUtility.HtmlDecode(href)}";
                        url = this.GetUrl(url);
                        var docNumber = this.ExtractDocumentNumber(url);
                        var nodeRaw = Encoding.UTF8.GetBytes(node.OuterHtml);

                        DocumentModel xmlDocument = null;
                        var documentInfo = this.ExtractDocumentInfo(node);

                        try
                        {
                            var searchedUrl = MainRechtsprechungUrl + this.GetQueryString(documentInfo);
                            var searchedPage = await this.Http.GetAsync(searchedUrl);
                            var searchedPageHtml = searchedPage.GetAsHtml();

                            var link = searchedPageHtml.DocumentNode.SelectSingleNode("//a[@id='tlid1']").Attributes["href"]?.Value;
                            var id = this.ExtractId(link);

                            var xmlPageUrl = this.GetXmlPageUrl(id);
                            var xmlPage = await this.Http.GetAsync(xmlPageUrl);
                            var xmlLink = this.ExtractXmlLink(xmlPage);
                            var xml = await this.Http.GetAsync($"{Rechtsprechung}{xmlLink}");

                            xmlDocument = new DocumentModel
                            {
                                Format = xml.MimeType,
                                Url = xml.Url,
                                Raw = xml.Raw,
                                Name = $"{id}.zip"
                            };
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error($"XML was not downloaded: {documentInfo}", ex);
                        }

                        try
                        {
                            var nodeDocument = new DocumentModel
                            {
                                Format = page.MimeType,
                                Url = page.Url,
                                Raw = nodeRaw,
                                Name = $"{docNumber}_tr.html"
                            };

                            var document = await this.Http.GetAsync(url);
                            var documentModel = this.GetDocumentModel(document, docNumber);
                            var documentGroup = this.GetDocumentGroup(docNumber, nodeDocument, documentModel, xmlDocument);

                            var isDownloadGroup = this.DocumentGroupManager.AddOrUpdateDocumentGroup(documentGroup);

                            if (isDownloadGroup)
                            {
                                this.Logger.Info($"Downloaded document package: {mainUrl}{href}");
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error($"Download document Url: {url}", ex);
                        }
                    }
                }
            }
        }
        private string ExtractDocumentInfo(HtmlNode node)
        {
            string documentInfo = null;
            var match = Regex.Match(node.OuterHtml, @"(?:'doklink'|'Entscheidung anzeigen'|false;')>(.*?)<\/a>");
            if (match.Success)
            {
                documentInfo = match.Groups[1].Value.Trim();
            }
            else
            {
                var secondMatch = Regex.Match(node.OuterHtml, @"'EAz'>(.*?)<br", RegexOptions.IgnoreCase);
                if (secondMatch.Success)
                {
                    documentInfo = secondMatch.Groups[1].Value.Trim();
                }
            }

            return WebUtility.HtmlDecode(documentInfo);
        }

        private DocumentGroupModel GetDocumentGroup(string docNumber, DocumentModel nodeDocument, DocumentModel documentModel, DocumentModel xmlDocument)
        {
            var documents = new List<DocumentModel> { nodeDocument, documentModel };
            if (xmlDocument != null)
            {
                documents.Add(xmlDocument);
            }

            var documentGroup = new DocumentGroupModel
            {
                CrawlerId = this.CrawlerId,
                TwoLetterLanguage = "DE",
                Name = docNumber,
                Documents = documents
            };

            return documentGroup;
        }

        private string ExtractXmlLink(HttpModel xmlPage)
        {
            var html = xmlPage.GetAsHtml();
            var xmlLink = html.DocumentNode.SelectSingleNode("//a[@name='XML']").Attributes["href"]?.Value;
            return xmlLink;
        }

        private string GetXmlPageUrl(string id)
        {
            var url = $"{BaseRechtsprechungUrl}" +
                $"&doc.id={id}" +
                $"&documentnumber=1" +
                $"&numberofresults=1" +
                $"&doctyp=juris-r" +
                $"&showdoccase=1" +
                $"&doc.part=K" +
                $"&paramfromHL=true";

            return url;
        }

        private string ExtractId(string link)
        {
            var match = Regex.Match(link, @"doc\.id=(.*?)&amp", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return null;
        }

        private string GetQueryString(string documentInfo)
        {
            var data = $"formhaschangedvalue=yes" +
                $"&eventSubmit_doSearch=suchen" +
                $"&action=portlets.jw.MainAction" +
                $"&deletemask=no" +
                $"&wt_form=1" +
                $"&form=bsjrsFastSearch" +
                $"&sugline=-1" +
                $"&sugstart={documentInfo}" +
                $"&sugcountrows=10" +
                $"&sugshownorelevanz=false" +
                $"&sugactive=true" +
                $"&sugportal=ETMsDgAAAWrTvk89ABRBRVMvQ0JDL1BLQ1M1UGFkZGluZwCAABAAEI0SH1y4icaZnz7XL8pwXIwAAABAtxFrqAoaizdyCjItBZe1v5rctT1RcQYxzBbRnJVfSNNSTkhWK0bx9iTjH9XWtd7uLtwQXIKahghc9qrn2DsHxwAUTcTShZb94KDACkpLdb4dhFiUABA%3D" +
                $"&sugportalport=8080" +
                $"&sughashcode=719956630395769026537894064269743437290001" +
                $"&sugwebhashcode=" +
                $"&sugcmspath=%2Fjportal%2Fcms%2F" +
                $"&desc=all" +
                $"&sug_all=" +
                $"&query={documentInfo}" +
                $"&standardsuche=suchen";

            return data;
        }

        private string GetUrl(string url)
        {
            if (url.StartsWith("https://juris.bundesgerichtshof.de/cgi-bin/rechtsprechung/"))
            {
                return url + "&Frame=4&.pdf";
            }

            return url;
        }

        private DocumentModel GetDocumentModel(HttpModel document, string docNumber)
        {
            if (document.MimeType == "application/pdf")
            {
                return new DocumentModel
                {
                    Format = document.MimeType,
                    Url = document.Url,
                    Raw = document.Raw,
                    Name = docNumber + ".pdf"
                };
            }

            return new DocumentModel
            {
                Format = document.MimeType,
                Url = document.Url,
                Raw = document.Raw,
                Name = docNumber + ".html"
            };
        }

        private string ExtractHrefLink(string html)
        {
            var match = Regex.Match(html, @"href='(.*?)'", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        private string ExtractDocumentNumber(string url)
        {
            var match = Regex.Match(url, @"&nr=(\d+)");
            return match.Groups[1].Value;
        }

        private bool CheckIsNextpage(HtmlDocument htmlDocument)
        {
            var node = htmlDocument.DocumentNode.SelectSingleNode("//td[@class='pagenumber']/a/img[@name='img_p_right']");
            if (node == null)
            {
                return false;
            }

            return true;
        }
    }
}
