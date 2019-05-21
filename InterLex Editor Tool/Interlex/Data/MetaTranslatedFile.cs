namespace Interlex.Data
{
    public class MetaTranslatedFile
    {
        public int Id { get; set; }
        public int MetadataId { get; set; }
        public byte[] Content { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public virtual Metadata Metadata { get; set; }
    }
}