namespace Interlex.Crawler.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Operation status for <see cref="DocumentModel"/> used by the <see cref="Manager.DocumentGroupManager"/>
    /// </summary>
    public enum DocumentModelOperation
    {
        None,

        /// <summary>
        /// New document
        /// </summary>
        Add,

        /// <summary>
        /// Updated document
        /// </summary>
        Upd,

        /// <summary>
        /// Deleted document
        /// </summary>
        Del
    }
}
