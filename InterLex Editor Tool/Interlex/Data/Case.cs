using System;
using System.Collections.Generic;

namespace Interlex.Data
{
    public class Case : IMetaCase
    {
        public Case()
        {
            CasesLog = new HashSet<CaseLog>();
        }

        public int Id { get; set; }
        public string Content { get; set; }
        public string Caption { get; set; }
        public DateTime LastChange { get; set; }
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<CaseLog> CasesLog { get; set; }

        public bool? IsDeleted { get; set; }
    }
}