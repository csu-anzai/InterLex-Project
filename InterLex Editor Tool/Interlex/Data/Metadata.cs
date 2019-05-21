namespace Interlex.Data
{
    using System;
    using System.Collections.Generic;

    public class Metadata : IMetaCase
    {
        public Metadata()
        {
            MetadataLog = new HashSet<MetadataLog>();
            MetaFile = new HashSet<MetaFile>();
            MetaTranslatedFile = new HashSet<MetaTranslatedFile>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Caption { get; set; }
        public DateTime LastChange { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<MetadataLog> MetadataLog { get; set; }
        
        public virtual ICollection<MetaFile> MetaFile { get; set; }
        public virtual ICollection<MetaTranslatedFile> MetaTranslatedFile { get; set; }

    }

    
}