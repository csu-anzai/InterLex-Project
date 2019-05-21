namespace Interlex.Data
{
    using System;

    public class MetadataLog
    {
        public int Id { get; set; }
        public int MetadataId { get; set; }
        public string UserId { get; set; }
        public DateTime ChangeDate { get; set; }
        public string Content { get; set; }

        public virtual Metadata Metadata { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}