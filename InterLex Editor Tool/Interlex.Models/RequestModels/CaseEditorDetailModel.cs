using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.RequestModels
{
    public class CaseEditorDetailModel
    {
        public Nomenclature Jurisdiction { get; set; }

        public string Source { get; set; }

        public string Court { get; set; }

        public string CourtEng { get; set; }

        public string[] Keywords { get; set; }
    }
}
