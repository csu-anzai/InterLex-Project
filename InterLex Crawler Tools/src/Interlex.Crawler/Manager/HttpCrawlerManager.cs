namespace Interlex.Crawler.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using Interlex.Crawler.Common;
    using Interlex.Crawler.Crawlers;
    using Interlex.Crawler.Model;
    using log4net;

    /// <summary>
    /// Represents manager for the <see cref="Crawlers.BaseHttpCrawler"/> crawlers
    /// </summary>
    public class HttpCrawlerManager
    {
        private readonly ILog logger;
        private readonly DocumentGroupManager documentGroupManager;
        private readonly CrawlerHttpManagerConfig config;
        private readonly HashSet<String> processedUrls;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="config">Confg</param>
        public HttpCrawlerManager(ILog logger, CrawlerHttpManagerConfig config)
        {
            this.logger = logger;
            this.documentGroupManager = new DocumentGroupManager();
            this.processedUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Starts the crawling, creating and persisting of the packages to the database
        /// </summary>
        /// <param name="crawlers">Crawlers to start</param>
        /// <returns></returns>
        public async Task StartAsync(IReadOnlyCollection<BaseHttpCrawler> crawlers)
        {
            foreach (var crawler in crawlers)
            {
                await this.StartAsync(crawler);
            }
        }


        /// <summary>
        /// Starts the crawling, creating and persisting of the packages to the database
        /// </summary>
        /// <param name="crawler">Crawler to start</param>
        /// <returns></returns>
        public async Task StartAsync(BaseHttpCrawler crawler)
        {
            // todo: protect agains recursive calls link1 -> link2 -> link1

            var crawlerName = crawler.GetType().FullName;
            var crawlerId = this.documentGroupManager.GetOrCreateCrawlerId(crawlerName);

            this.logger.Info($"Start {crawlerName} - {DateTime.UtcNow}");

            foreach (var initialPageUrl in crawler.InitialPageUrls)
            {
                var packages = await this.CrawlAsyncRecursivly(crawler, initialPageUrl, crawlerId);
            }

            this.logger.Info($"End {crawlerName} - {DateTime.UtcNow}");
        }

        private async Task<IReadOnlyCollection<CrawlerDownloadPackageModel>> CrawlAsyncRecursivly(BaseHttpCrawler crawler, String url, int crawlerId)
        {
            var result = new List<CrawlerDownloadPackageModel>();

            try
            {
                var httpGet = await crawler.Http.GetAsync(url);
                var packageInfo = await this.CrawlAsyncRecursivly(crawler, httpGet, null, crawlerId);

                // with the current impl. of the CrawlAsyncRecursivly the last package could be skipped and not pushed in the database
                // depending on the recursion level from which the package was downloaded 
                // so we must explicitly check and process it
                var notProcessed = packageInfo.Where(x => x.Success && x.IsProcessed == false).SingleOrDefault();
                if (notProcessed != null)
                {
                    await this.AddOrUpdatePackageToDatabase(crawler, notProcessed, crawlerId);
                }

                result.AddRange(packageInfo);

                // todo: log or send notification view email or something like this for the package info
                // probably how many has failed / successed .. etc
            }
            catch (Exception e)
            {
                this.logger.Error(message: url, exception: e);
            }

            return result;
        }

        private async Task<IReadOnlyCollection<CrawlerDownloadPackageModel>> CrawlAsyncRecursivly(BaseHttpCrawler crawler, HttpModel parentHttpGet, HtmlNode parentAnchor, int crawlerId)
        {
            var packages = new List<CrawlerDownloadPackageModel>();
            var package = (CrawlerDownloadPackageModel)null;

            var anchors = GetAnchors(parentHttpGet, crawler);

            foreach (var anchor in anchors)
            {
                var httpGet = (HttpModel)null;
                
                // if the url is not already processed (protection against infinity recursive call)
                if (this.processedUrls.Add(anchor.Href()))
                {
                    try
                    {
                        if (crawler.IsPackage(anchor))
                        {
                            // process current package before create new
                            if (package?.Success == true && package?.IsProcessed == false)
                            {
                                await this.AddOrUpdatePackageToDatabase(crawler, package, crawlerId);
                            }

                            (package, httpGet) = await this.CreatePackage(crawler, anchor, parentHttpGet, parentAnchor);
                            packages.Add(package);
                        }
                        // if package has failed we should skip processing documents for the failed package
                        else if (crawler.IsPackageDocument(anchor) && package?.Success == true)
                        {
                            // if given package is already processed we should not attach additional documents to the package
                            if (package.IsProcessed)
                            {
                                throw new InvalidOperationException($"Not allowed to add documents to already processed package. Name: {package.Name}; Url: {anchor.Href()}");
                            }

                            httpGet = await crawler.Http.GetAsync(anchor.Href());
                            package.Documents.Add(CreatePackgeDocument(crawler, httpGet, anchor));

                            this.logger.Info($"Downloaded document package: {anchor.Href()}");
                        }

                        if (crawler.ShouldExamine(anchor))
                        {
                            if (httpGet == null)
                            {
                                httpGet = await crawler.Http.GetAsync(anchor.Href());
                            }

                            this.logger.Debug($"Going recursivly for: {anchor.Href()}");

                            packages.AddRange(await this.CrawlAsyncRecursivly(crawler, httpGet, anchor, crawlerId));
                            // process if there is any package
                            await this.AddOrUpdatePackageToDatabase(crawler, packages, crawlerId);
                        }
                        // when not processed
                        else if (httpGet == null)
                        {
                            if (crawler.IsPackageDocument(anchor) && package?.Success == false)
                            {
                                this.logger.Debug($"Package failed -> skip downloading document: {anchor.Href()}");
                            }
                            else
                            {
                                this.logger.Debug($"Skip downloading: {anchor.Href()}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        package = CrawlerDownloadPackageModel.CreateFailed(e.ToString());
                        packages.Add(package);

                        this.logger.Error(message: $"{anchor.Href()}", e);
                    }
                }
            }

            return packages;
        }

        private async Task<(CrawlerDownloadPackageModel package, HttpModel httpGet)> CreatePackage(BaseHttpCrawler crawler, HtmlNode anchor, HttpModel parentHttpGet, HtmlNode parentAnchor)
        {
            var httpGet = await crawler.Http.GetAsync(anchor.Href());
            var package = CreatePackge(crawler, httpGet, anchor);

            if (crawler.ShouldIncludePackgeAsDocument(anchor))
            {
                // the package itself is also treated as package document (if this needs to be changed just introduce config option to control the behaviour)
                package.Documents.Add(CreatePackgeDocument(crawler, httpGet, anchor));
            }

            this.logger.Info($"Downloaded package: {package.Url}");

            if (crawler.ShouldIncludeParentPageAsDocument(parentAnchor, anchor))
            {
                package.Documents.Add(CreatePackgeDocument(crawler, parentHttpGet, parentAnchor));
            }

            return (package, httpGet);
        }

        private async Task AddOrUpdatePackageToDatabase(BaseHttpCrawler crawler, CrawlerDownloadPackageModel package, int crawlerId)
        {
            var documentGroupModel = new DocumentGroupModel
            {
                Name = package.Name.Value,
                TwoLetterLanguage = package.TwoLetterLanguage,
                CrawlerId = crawlerId
            };

            var failed = false;
            foreach (var downloadModel in package.Documents)
            {
                try
                {
                    var httpGet = downloadModel.HttpGet;
                    if (downloadModel.IsDownloaded == failed)
                    {
                        httpGet = await crawler.Http.GetAsync(downloadModel.Url);
                    }

                    documentGroupModel.Documents.Add(new DocumentModel
                    {
                        Format = httpGet.MimeType,
                        Raw = httpGet.Raw,
                        Name = downloadModel.Name.Value,
                        Url = downloadModel.Url,
                    });
                }
                catch (Exception e)
                {
                    this.logger.Error(message: $"package name: {package.Name}; url: {downloadModel.Url}", exception: e);
                    failed = true;
                    break;
                }
            }

            package.IsProcessed = true;
            package.FreeResource();
            this.logger.Info($"Processed package: {package.Url}");

            if (failed == false)
            {
                //var isNewOrUpdated = this.documentGroupManager.AddOrUpdateDocumentGroup(documentGroupModel);
                //if (isNewOrUpdated)
                //{
                //    this.logger.Info(message: $"New or updated: {documentGroupModel.Name}; {documentGroupModel.Identifier}");
                //}
            }
        }

        private async Task AddOrUpdatePackageToDatabase(BaseHttpCrawler crawler, IReadOnlyCollection<CrawlerDownloadPackageModel> packages, int crawlerId)
        {
            foreach (var package in packages.Where(x => x.IsProcessed == false && x.Success == true))
            {
                await this.AddOrUpdatePackageToDatabase(crawler, package, crawlerId);
            }
        }

        private static CrawlerDownloadPackageModel CreatePackge(BaseHttpCrawler crawler, HttpModel httpGet, HtmlNode anchor)
        {
            var name = crawler.CreatePackageName(anchor, httpGet);
            var twoLetterLang = crawler.GetTwoLetterLanguage(httpGet, anchor);

            return CrawlerDownloadPackageModel.CreateSuccess(name, httpGet.Encoding, anchor.Href(), twoLetterLang);
        }

        private static CrawlerDownloadDocumentModel CreatePackgeDocument(BaseHttpCrawler crawler, HttpModel httpGet, HtmlNode anchor)
        {
            var name = crawler.CreatePackageDocumentName(anchor, httpGet);
            return new CrawlerDownloadDocumentModel(name, anchor.Href(), httpGet);
        }

        private static IReadOnlyCollection<HtmlNode> GetAnchors(HttpModel httpGet, BaseHttpCrawler crawler)
        {
            var html = httpGet.GetAsHtml();
            var anchors = html.DocumentNode.SelectNodes(".//a").ToList();
            var resultAnchors = anchors.ToList();
            foreach (var anchor in anchors)
            {
                var href = anchor.Href();
                var baseUri = new Uri(httpGet.Url);

                var uri = new Uri(baseUri, href);
                if (String.IsNullOrEmpty(uri.Fragment) == false && uri.Fragment.StartsWith("#"))
                {
                    // hrefs with anchors (internal) should be unified because the anchors don't represent
                    // new url - they represent just location in the same document
                    // example: www.google.com#main -> www.google.com
                    var (urlWithoutFragment, _) = uri.AbsoluteUri.TupleSplit("#");
                    uri = new Uri(urlWithoutFragment);
                }

                anchor.SetAttributeValue("href", uri.AbsoluteUri.HtmlDecode());

                var additionalUrls = crawler.GenerateAdditionalUrls(anchor, null);
                resultAnchors.AddRange(additionalUrls.Select(x => HtmlNode.CreateNode($"<a href='{x}'></a>")));
            }

            // add and the caller as anchor
            var callerAnchor = html.CreateElement("a");
            callerAnchor.SetAttributeValue("href", httpGet.Url);
            resultAnchors.Add(callerAnchor);

            return resultAnchors;
        }
    }
}
