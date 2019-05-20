namespace Interlex.Crawler.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents model for the <see cref="Data.InterlexCrawlerEntities.DocumentGroup"/> entity.
    /// Use by the <see cref="Manager.DocumentGroupManager"/> when insert or update document groups.
    /// </summary>
    public class DocumentGroupModel
    {
        /// <summary>
        /// Identifier of the document group
        /// </summary>
        public String Identifier { get; set; }

        /// <summary>
        /// Name of the document group
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Two letter language
        /// </summary>
        public String TwoLetterLanguage { get; set; }

        /// <summary>
        /// Crawler id
        /// </summary>
        public int CrawlerId { get; set; }

        /// <summary>
        /// Documents
        /// </summary>
        public List<DocumentModel> Documents { get; set; } = new List<DocumentModel>();
    }
}
