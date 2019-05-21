namespace Interlex.Crawler.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Model for downloaded package
    /// </summary>
    [DebuggerDisplay("{DebugInfo()}")]
    public class CrawlerDownloadPackageModel
    {
        /// <summary>
        /// Empty sequence
        /// </summary>
        public static IReadOnlyCollection<CrawlerDownloadPackageModel> EmptySequence = Array.Empty<CrawlerDownloadPackageModel>();

        /// <summary>
        /// Creates package representing successful download
        /// </summary>
        /// <param name="name">Name of the package</param>
        /// <param name="encoding">Encofing of the package</param>
        /// <param name="url">Url of the package</param>
        /// <param name="twoLetterLanguage">Two letter language for the package</param>
        /// <returns></returns>
        public static CrawlerDownloadPackageModel CreateSuccess(NameModel name, Encoding encoding, String url, String twoLetterLanguage)
        {
            return new CrawlerDownloadPackageModel(name, encoding, url, twoLetterLanguage);
        }

        /// <summary>
        /// Creates package representing failed download
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns></returns>
        public static CrawlerDownloadPackageModel CreateFailed(String error)
        {
            return new CrawlerDownloadPackageModel(error);
        }

        /// <summary>
        /// Creates new instance representing successful package
        /// </summary>
        /// <param name="name">Name of the package</param>
        /// <param name="encoding">Encoding of the package</param>
        /// <param name="url">Url of the package</param>
        /// <param name="twoLetterLanguage">Two letter language</param>
        private CrawlerDownloadPackageModel(NameModel name, Encoding encoding, String url, String twoLetterLanguage)
        {
            this.Name = name;
            this.Encoding = encoding;
            this.Url = url;
            this.TwoLetterLanguage = twoLetterLanguage;
        }

        /// <summary>
        /// Creates new instance representing failed package
        /// </summary>
        /// <param name="error">Error message</param>
        private CrawlerDownloadPackageModel(String error)
        {
            this.Error = error;
            this.Success = String.IsNullOrEmpty(error);
        }

        /// <summary>
        /// True if the package was successfully downloaded
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Error message for failed package
        /// </summary>
        public String Error { get; }

        /// <summary>
        /// Name of the package
        /// </summary>
        public NameModel Name { get; }

        /// <summary>
        /// Encoding for the package
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// Two letter language
        /// </summary>
        public String TwoLetterLanguage { get; }

        /// <summary>
        /// Indicates if the package was processed by the <see cref="Manager.DocumentGroupManager"/>
        /// </summary>
        public bool IsProcessed { get; set; }

        /// <summary>
        /// Url of the package
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// Documents for the package
        /// </summary>
        public List<CrawlerDownloadDocumentModel> Documents { get; } = new List<CrawlerDownloadDocumentModel>();

        /// <summary>
        /// Frees any holded references (like the <see cref="CrawlerDownloadDocumentModel.HttpGet"/> propery of the <see cref="Documents"/> collection)
        /// </summary>
        public void FreeResource()
        {
            foreach (var document in this.Documents)
            {
                document.FreeResource();
            }
        }

#if DEBUG

        public String DebugInfo()
        {
            return $"Name: {this.Name.Value}; Download count: {this.Documents.Count}";
        }
#endif
    }
}
