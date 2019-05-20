using System;

namespace Interlex.Data
{
    public partial class CaseLog
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public string UserId { get; set; }
        public DateTime ChangeDate { get; set; }
        public string Content { get; set; }

        public Case Case { get; set; }
        public ApplicationUser User { get; set; }
    }
}
