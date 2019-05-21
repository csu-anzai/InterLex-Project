namespace Interlex.Data
{
    using System;

    public interface IMetaCase
    {
        int Id { get; set; }
        string UserId { get; set; }
        string Content { get; set; }
        string Caption { get; set; }
        DateTime LastChange { get; set; }
        bool? IsDeleted { get; set; }
        ApplicationUser User { get; set; }
    }
}