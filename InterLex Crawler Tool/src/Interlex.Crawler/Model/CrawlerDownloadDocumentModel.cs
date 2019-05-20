namespace Interlex.Crawler.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Model for downloaded document
    /// </summary>
    [DebuggerDisplay("{DebugInfo()}")]
    public class CrawlerDownloadDocumentModel
    {
        /// <summary>
        /// Empty sequence
        /// </summary>
        public static IReadOnlyCollection<CrawlerDownloadDocumentModel> EmptySequence = Array.Empty<CrawlerDownloadDocumentModel>();

        /// <summary>
        /// Creates new document
        /// </summary>
        /// <param name="name">Name of the document</param>
        /// <param name="url">Url of the document</param>
        /// <param name="httpGet">Http response for the <paramref name="url"/></param>
        /// <returns></returns>
        public static CrawlerDownloadDocumentModel Create(String name, String url, HttpModel httpGet)
        {
            return new CrawlerDownloadDocumentModel(NameModel.Create(name), url, httpGet);
        }

        /// <summary>
        /// Creates new document
        /// </summary>
        /// <param name="name">Name of the document</param>
        /// <param name="url">Url of the document</param>
        /// <param name="httpGet">Http response for the <paramref name="url"/></param>
        /// <returns></returns>
        public static CrawlerDownloadDocumentModel Create(NameModel name, String url, HttpModel httpGet)
        {
            return new CrawlerDownloadDocumentModel(name, url, httpGet);
        }

        /// <summary>
        /// Name of the document
        /// </summary>
        public NameModel Name { get; }

        /// <summary>
        /// Url of the document
        /// </summary>
        public String Url { get; }

        /// <summary>
        /// Http response
        /// </summary>
        public HttpModel HttpGet { get; private set; }

        /// <summary>
        /// True if the <see cref="HttpGet"/> is not null
        /// </summary>
        public bool IsDownloaded => this.HttpGet != null;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="name">Name of the document</param>
        /// <param name="url">Url of the document</param>
        /// <param name="httpGet">Http response for <paramref name="url"/></param>
        public CrawlerDownloadDocumentModel(NameModel name, String url, HttpModel httpGet)
        {
            this.Name = name;
            this.Url = url;
            this.HttpGet = httpGet;
        }

        /// <summary>
        /// Free any resources holded as references (like the <see cref="HttpGet"/> property)
        /// </summary>
        public void FreeResource() => this.HttpGet = null;

#if DEBUG
        public String DebugInfo()
        {
            return $"Name: {this.Name.Value}; Url: {this.Url}";
        }
#endif
    }
}
