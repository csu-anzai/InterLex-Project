namespace Interlex.Data
{
    using System;

    public partial class Relationship
    {
        public Guid ParentId { get; set; }
        public Guid ChildId { get; set; }

        public Codes Child { get; set; }
        public Codes Parent { get; set; }
    }
}
