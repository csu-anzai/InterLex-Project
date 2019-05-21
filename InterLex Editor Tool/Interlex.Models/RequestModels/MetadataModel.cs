namespace Interlex.Models.RequestModels
{
    public class MetadataModel : CaseModel
    {
        public FileModel File { get; set; }

        public FileModel TranslatedFile { get; set; }
    }

    public class FileModel
    {
        public string Base64Content { get; set; }

        public string Filename { get; set; }

        public string MimeType { get; set; }
    }
}