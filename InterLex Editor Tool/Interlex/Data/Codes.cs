namespace Interlex.Data
{
    using System;
    using System.Collections.Generic;
    using Models;

    public partial class Codes
    {
        public Codes()
        {
            Parents = new HashSet<Relationship>();
            Children = new HashSet<Relationship>();
            Texts = new HashSet<Texts>();
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }

        public ICollection<Relationship> Parents { get; set; }  // these are not direct mappings, you need to use Theninclude(x => x.Parent) !!!
        public ICollection<Relationship> Children { get; set; }
        public ICollection<Texts> Texts { get; set; }
    }
}
