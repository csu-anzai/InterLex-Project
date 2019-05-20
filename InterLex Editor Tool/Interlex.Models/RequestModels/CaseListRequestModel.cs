using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.RequestModels
{
    public class CaseListRequestModel
    {
        public string UserName { get; set; }
        public string PageNumber { get; set; }
        public string Count { get; set; }
        public string Organization { get; set; }

        public string JurisdictionCode { get; set; }
    }
}
