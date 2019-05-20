namespace Interlex.Crawler.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents model for the <see cref="Data.InterlexCrawlerEntities.Document"/> entity.
    /// Use by the <see cref="Manager.DocumentGroupManager"/> when insert or update document groups.
    /// </summary>
    public class DocumentModel
    {
        /// <summary>
        /// Identifier of the document
        /// </summary>
        public String Identifier { get; set; }

        /// <summary>
        /// Name of the document
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Content of the document
        /// </summary>
        public Byte[] Raw { get; set; }

        /// <summary>
        /// Md5 hash of the <see cref="Raw"/> property
        /// </summary>
        public String Md5 { get; set; }

        /// <summary>
        /// Format of the document group (Mime type)
        /// </summary>
        public String Format { get; set; }

        /// <summary>
        /// Url of the document
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// Operation for the document
        /// </summary>
        public DocumentModelOperation Operation { get; set; }
    }
}
