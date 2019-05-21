namespace Interlex.Crawler.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Operation status for specific document group when processed by the <see cref="Manager.DocumentGroupManager"/>
    /// </summary>
    public enum DocumentGroupModelOperation
    {
        /// <summary>
        /// New document group
        /// </summary>
        Add,

        /// <summary>
        /// Document group for update
        /// </summary>
        Upd
    }
}
