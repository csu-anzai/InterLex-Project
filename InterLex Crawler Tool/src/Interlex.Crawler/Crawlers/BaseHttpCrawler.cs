namespace Interlex.Crawler.Crawlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using Interlex.Crawler.Common;
    using Interlex.Crawler.Model;
    using log4net;

    /// <summary>
    /// Represents base crawler used in conjution with the <see cref="Manager.HttpCrawlerManager"/>.
    /// Suitable for more generic crawling where the structure of the <see cref="Model.DocumentGroupModel"/> is constructed from single document.
    /// The resposabilities of the crawler is to determ if given url is packge or document for the packge.
    /// </summary>
    public abstract class BaseHttpCrawler
    {
        internal readonly Http Http;
        internal readonly IReadOnlyCollection<String> InitialPageUrls;
        protected readonly ILog Logger;

        /// <summary>
        /// Creates new intance of the crawler
        /// </summary>
        /// <param name="http">Http requester</param>
        /// <param name="initialPageUrl">Url to start the crawling from</param>
        /// <param name="logger">Logger</param>
        public BaseHttpCrawler(Http http, String initialPageUrl, ILog logger) : this(http, new[] { initialPageUrl }, logger)
        {
        }

        /// <summary>
        /// Creates new instance of the crawler
        /// </summary>
        /// <param name="http">Http requester</param>
        /// <param name="initialPageUrls">Urls to start the crawling from</param>
        /// <param name="logger">Logger</param>
        public BaseHttpCrawler(Http http, IReadOnlyCollection<String> initialPageUrls, ILog logger)
        {
            this.Http = http;
            this.InitialPageUrls = initialPageUrls;
            this.Logger = logger;
        }

        /// <summary>
        /// Determs if the given http response is not found (404)
        /// </summary>
        /// <param name="httpGet"></param>
        /// <returns></returns>
        public abstract bool IsNotFound(HttpModel httpGet);

        /// <summary>
        /// Determs if given anchor is package
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public abstract bool IsPackage(HtmlNode anchor);

        /// <summary>
        /// Determs if the anchor is document package
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public abstract bool IsPackageDocument(HtmlNode anchor);

        /// <summary>
        /// Determs if the given url should be crawled and furtuer examined for packages and document packages.
        /// Example: the next page anchor of result search in given website is appropraite to be examined for additional packages and document packages
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public abstract bool ShouldExamine(HtmlNode anchor);

        /// <summary>
        /// Create unique name for the package.
        /// The name of the package is the identity used for representing specific package in the database.
        /// Checking in the database if the package is new or it should update already persisted package in the database is determed by the name of the package.
        /// </summary>
        /// <param name="anchor">Anchor of the package</param>
        /// <param name="httpGet">Http reponse of the download webpage from the href of the <paramref name="anchor"/></param>
        /// <returns></returns>
        public abstract NameModel CreatePackageName(HtmlNode anchor, HttpModel httpGet);

        /// <summary>
        /// Create unique name for the document in given package.
        /// The name of the document should unique in the context of the package.
        /// </summary>
        /// <param name="anchor">Anchor of the document package</param>
        /// <param name="httpGet">Http reponse of the download webpage from the href of the <paramref name="anchor"/></param>
        /// <returns></returns>
        public abstract NameModel CreatePackageDocumentName(HtmlNode anchor, HttpModel httpGet);

        /// <summary>
        /// Creates two letter language for the specific package anchor
        /// </summary>
        /// <param name="packageHttpGet">Http response for specific package</param>
        /// <param name="packageAnchor">Anchor for specific package</param>
        /// <returns></returns>
        public abstract String GetTwoLetterLanguage(HttpModel packageHttpGet, HtmlNode packageAnchor);

        /// <summary>
        /// Determs if the parent page of the anchor should included as package document for the specific package. Default is false.
        /// </summary>
        /// <param name="parentPageAnchor">Anchor for the parent page</param>
        /// <param name="currentAnchor">Anchor for the current package</param>
        /// <returns></returns>
        public virtual bool ShouldIncludeParentPageAsDocument(HtmlNode parentPageAnchor, HtmlNode currentAnchor)
        {
            return false;
        }

        /// <summary>
        /// Determs if the package content should be included as package document. Default is true.
        /// </summary>
        /// <param name="packageAnchor"></param>
        /// <returns></returns>
        public virtual bool ShouldIncludePackgeAsDocument(HtmlNode packageAnchor)
        {
            return true;
        }

        /// <summary>
        /// Returns additional urls to be processed
        /// </summary>
        /// <param name="anchor">Anchor</param>
        /// <param name="httpModel">Http response of the <paramref name="anchor"/></param>
        /// <returns></returns>
        public virtual IReadOnlyCollection<String> GenerateAdditionalUrls(HtmlNode anchor, HttpModel httpModel)
        {
            return Array.Empty<String>();
        }
    }
}
