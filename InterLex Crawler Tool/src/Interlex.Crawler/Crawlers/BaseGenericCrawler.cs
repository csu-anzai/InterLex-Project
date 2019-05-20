namespace Interlex.Crawler.Crawlers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Interlex.Crawler.Common;
    using Interlex.Crawler.Manager;
    using log4net;

    /// <summary>
    /// Represents base class for crawlers with more control over the process of crawling.
    /// Suitable for complex websites where specific <see cref="Model.DocumentGroupModel"/> is construced from several documents
    /// </summary>
    public abstract class BaseGenericCrawler
    {
        protected DocumentGroupManager DocumentGroupManager;
        protected Http Http;
        protected ILog Logger;
        protected int CrawlerId;

        /// <summary>
        /// Creates new instance of the crawler
        /// </summary>
        /// <param name="logger">Logger</param>
        public BaseGenericCrawler(ILog logger)
        {
            this.Logger = logger;
            this.DocumentGroupManager = new DocumentGroupManager();
            this.Http = new Http();
            this.CrawlerId = this.DocumentGroupManager.GetOrCreateCrawlerId(this.GetType().FullName);
        }


        /// <summary>
        /// Starts the crawling process. In derived class this method is resposible for the crawling of the documents and the storage of the documents into the database using the <see cref="DocumentGroupManager"/> class.
        /// </summary>
        /// <returns></returns>
        public abstract Task StartAsync();
    }
}
