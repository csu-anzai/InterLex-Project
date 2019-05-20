namespace Interlex.Data
{
    using System;

    public partial class Texts
    {
        public int Id { get; set; }
        public Guid ClassifierId { get; set; }
        public string Text { get; set; }
        public int LanguageId { get; set; }

        public Codes Classifier { get; set; }
        public Language Language { get; set; }
    }
}
