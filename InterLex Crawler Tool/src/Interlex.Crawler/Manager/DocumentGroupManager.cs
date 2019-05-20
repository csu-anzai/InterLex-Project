using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interlex.Crawler.Common;
using Interlex.Crawler.Data;
using Interlex.Crawler.Model;
using Microsoft.EntityFrameworkCore;

namespace Interlex.Crawler.Manager
{
    /// <summary>
    /// Represents class providing database persistance capabilities for document groups
    /// </summary>
    public class DocumentGroupManager
    {
        private static object lockObject = new object();

        /// <summary>
        /// Creates new instance
        /// </summary>
        public DocumentGroupManager()
        {
        }


        /// <summary>
        /// Add or update the speicified document group to the database using the <see cref="DocumentGroupModel.Name"/> property to match if the document group exists in the database
        /// </summary>
        /// <param name="crawledDocumentGroup"></param>
        /// <returns></returns>
        public bool AddOrUpdateDocumentGroup(DocumentGroupModel crawledDocumentGroup)
        {
            ValidateDocumentNames(crawledDocumentGroup);

            foreach (var crawledDocument in crawledDocumentGroup.Documents)
            {
                crawledDocument.Md5 = MD5Hash.GetMd5Hash(crawledDocument.Raw);
                crawledDocument.Identifier = Guid.NewGuid().ToString();
                crawledDocument.Name = crawledDocument.Name.ToLower();
            }

            var folderName = crawledDocumentGroup.Name.ToLower();
            var zipFileName = folderName + ".zip";
            crawledDocumentGroup.Name = zipFileName;

            return this.ProcessDocumentGroup(crawledDocumentGroup);
        }


        /// <summary>
        /// Returns the crawler id with the specified name. If the crawler does not exists creates the crawler in the database and returns the id
        /// </summary>
        /// <param name="crawlerName"></param>
        /// <returns></returns>
        public int GetOrCreateCrawlerId(string crawlerName)
        {
            using (var context = new InterlexCrawlerEntities())
            {
                lock (lockObject)
                {
                    var crawler = (from c in context.Crawlers
                                   where c.CrawlerName == crawlerName
                                   select c.CrawlerId).FirstOrDefault();

                    if (crawler == 0)
                    {
                        var newCrawler = new InterlexCrawlerEntities.Crawler();
                        newCrawler.CrawlerName = crawlerName;
                        context.Crawlers.Add(newCrawler);
                        context.SaveChanges();

                        return this.GetOrCreateCrawlerId(crawlerName);
                    }

                    return crawler;
                }
            }
        }

        private bool ProcessDocumentGroup(DocumentGroupModel crawledDocumentGroup)
        {
            var isProcessed = false;

            var databaseDocumentGroup = this.GetDocumentGroupInfo(crawledDocumentGroup.CrawlerId, crawledDocumentGroup.Name);

            if (databaseDocumentGroup != null)
            {
                var isUpdated = this.IsUpdated(crawledDocumentGroup, databaseDocumentGroup);

                if (isUpdated)
                {
                    this.UpdateDocumentGroup(crawledDocumentGroup, databaseDocumentGroup);
                    isProcessed = true;
                }
            }
            else
            {
                this.AddDocumentGroup(crawledDocumentGroup);
                isProcessed = true;
            }

            return isProcessed;
        }

        private DocumentGroupModel GetDocumentGroupInfo(int cralwerId, string documentGroupName)
        {
            using (var context = new InterlexCrawlerEntities())
            {
                var info = context.DocumentGroups
                     .Include(x => x.Documents)
                     .Where(x => x.CrawlerId == cralwerId && x.DocumentGroupName == documentGroupName)
                     .Select(x => new DocumentGroupModel
                     {
                         Name = x.DocumentGroupName,
                         Identifier = x.Identifier,
                         Documents = x.Documents.Select(d => new DocumentModel
                         {
                             Name = d.DocumentName,
                             Md5 = d.Md5,
                             Operation = (DocumentModelOperation)d.Operation,
                             Format = d.DocumentFormat
                         }).ToList()
                     }).FirstOrDefault();

                return info;
            }
        }

        private void UpdateDocumentGroup(DocumentGroupModel crawledDocumentGroup, DocumentGroupModel documentGroupFromDatabase)
        {
            using (var context = new InterlexCrawlerEntities())
            {
                var documentGroupDb = (from dg in context.DocumentGroups
                                       where dg.Identifier == documentGroupFromDatabase.Identifier
                                       select dg).Single();

                context.Entry(documentGroupDb).Collection(x => x.Documents).Load();

                documentGroupDb.Operation = (int)DocumentGroupModelOperation.Upd;
                documentGroupDb.DataContent = Zip.DocumentGroup(crawledDocumentGroup);
                documentGroupDb.DocumentGroupDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
                documentGroupDb.Lang = crawledDocumentGroup.TwoLetterLanguage;

                var documentOrder = 0;
                foreach (var crawledDocument in crawledDocumentGroup.Documents)
                {
                    if (crawledDocument.Operation == DocumentModelOperation.Add)
                    {
                        string fileLower = crawledDocument.Name.ToLower();
                        var document = new InterlexCrawlerEntities.Document();
                        document.DocumentName = fileLower;
                        document.Identifier = crawledDocument.Identifier;
                        document.DocumentFormat = crawledDocument.Format;
                        document.Operation = (int)crawledDocument.Operation;
                        document.DocumentOrder = documentOrder;
                        document.Url = crawledDocument.Url;
                        document.Md5 = crawledDocument.Md5;

                        documentGroupDb.Documents.Add(document);
                    }
                    else if (crawledDocument.Operation == DocumentModelOperation.Upd)
                    {
                        var dbDocument = documentGroupDb.Documents.Where(x => x.DocumentName == crawledDocument.Name).FirstOrDefault();
                        dbDocument.Operation = (int)crawledDocument.Operation;
                        dbDocument.DocumentOrder = documentOrder;
                        dbDocument.DocumentFormat = crawledDocument.Format;
                        dbDocument.Url = crawledDocument.Url;
                        dbDocument.Md5 = crawledDocument.Md5;
                    }

                    documentOrder++;
                }

                foreach (var documenInfo in documentGroupFromDatabase.Documents)
                {
                    if (documenInfo.Operation == DocumentModelOperation.Del)
                    {
                        var dbDocument = documentGroupDb.Documents.Where(x => x.DocumentName == documenInfo.Name).FirstOrDefault();
                        if (dbDocument != null)
                        {
                            dbDocument.Operation = (int)documenInfo.Operation;
                        }
                    }
                }


                context.SaveChanges();

                context.PChangeOperationStatus(documentGroupDb.Identifier, (int)DocumentGroupModelOperation.Upd, "DatabaseDocumentManager", null);
            }
        }

        private bool IsUpdated(DocumentGroupModel crawledDocumentGroup, DocumentGroupModel documentGroupFromDatabase)
        {
            var isProcess = false;
            foreach (var crawledDocument in crawledDocumentGroup.Documents)
            {
                var documentInfo = documentGroupFromDatabase.Documents.Where(x => x.Name == crawledDocument.Name).FirstOrDefault();

                if (documentInfo != null)
                {
                    if (documentInfo.Md5 != crawledDocument.Md5 || documentInfo.Operation == DocumentModelOperation.Del || documentInfo.Format != crawledDocument.Format)
                    {
                        crawledDocument.Operation = DocumentModelOperation.Upd;
                        documentInfo.Operation = DocumentModelOperation.Upd;

                        isProcess = true;
                    }
                    else
                    {
                        crawledDocument.Operation = DocumentModelOperation.None;
                    }
                }
                else
                {
                    crawledDocument.Operation = DocumentModelOperation.Add;
                    isProcess = true;
                }
            }

            foreach (var documenInfo in documentGroupFromDatabase.Documents)
            {
                if (!crawledDocumentGroup.Documents.Where(x => x.Name == documenInfo.Name).Any())
                {
                    documenInfo.Operation = DocumentModelOperation.Del;
                    isProcess = true;
                }
            }

            return isProcess;
        }

        private void AddDocumentGroup(DocumentGroupModel crawledDocumentGroup)
        {
            var newDocumentGroup = new InterlexCrawlerEntities.DocumentGroup();
            newDocumentGroup.CrawlerId = crawledDocumentGroup.CrawlerId;
            newDocumentGroup.Identifier = Guid.NewGuid().ToString();
            newDocumentGroup.DocumentGroupName = crawledDocumentGroup.Name;
            newDocumentGroup.DocumentGroupFormat = "application/zip";
            newDocumentGroup.Lang = crawledDocumentGroup.TwoLetterLanguage;
            newDocumentGroup.Operation = (int)DocumentGroupModelOperation.Add;
            newDocumentGroup.DocumentGroupDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

            // Byte Array Data
            newDocumentGroup.DataContent = Zip.DocumentGroup(crawledDocumentGroup);
            this.AddDocuments(newDocumentGroup, crawledDocumentGroup);

            using (var context = new InterlexCrawlerEntities())
            {
                context.DocumentGroups.Add(newDocumentGroup);
                context.SaveChanges();
                context.PChangeOperationStatus(newDocumentGroup.Identifier, (int)DocumentGroupModelOperation.Add, "DatabaseDocumentManager", null);
            }
        }

        private void AddDocuments(InterlexCrawlerEntities.DocumentGroup newDocumentGroup, DocumentGroupModel crawleredDocumentGroup)
        {
            var documentOrder = 1;
            foreach (var crawledDocument in crawleredDocumentGroup.Documents)
            {
                var fileLower = crawledDocument.Name .ToLower();
                var document = new InterlexCrawlerEntities.Document();
                document.DocumentName = fileLower;
                document.Identifier = crawledDocument.Identifier;
                document.DocumentFormat = crawledDocument.Format;
                document.Operation = (int)crawledDocument.Operation;
                document.DocumentOrder = documentOrder;
                document.Url = crawledDocument.Url;
                document.Md5 = crawledDocument.Md5;

                newDocumentGroup.Documents.Add(document);

                documentOrder++;
            }
        }

        private static void ValidateDocumentNames(DocumentGroupModel crawledDocumentGroup)
        {
            // check that all names are unique
            var areUnique = crawledDocumentGroup.Documents.Count == crawledDocumentGroup.Documents.Select(x => x.Name).Distinct().Count();

            if (areUnique == false)
            {
                throw new ArgumentException($"Document names must be unique! Document group name={crawledDocumentGroup.Name}");
            }
        }
    }
}
