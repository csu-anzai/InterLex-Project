namespace Interlex.Data
{
    using System.Collections.Generic;
    using Models;

    public partial class Language
    {
        public Language()
        {
            Texts = new HashSet<Texts>();
        }

        public int Id { get; set; }
        public string TwoLetter { get; set; }
        public string ThreeLetter { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public ICollection<Texts> Texts { get; set; }
    }
}
