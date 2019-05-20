namespace Interlex.Models.ResponseModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("{Ecli}; {Title}")]
    public class ReferenceEditorActInfoModel
    {
        public String Title { get; set; }
        public String Ecli { get; set; }
        public DateTime? Date { get; set; }
    }
}
