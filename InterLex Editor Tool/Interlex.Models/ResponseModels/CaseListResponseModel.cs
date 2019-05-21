using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.ResponseModels
{
    public class CaseListResponseModel
    {
        public int CaseId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public DateTime LastChange { get; set; }

        public bool Editable { get; set; }
        public string Organization { get; set; }
        public DateTime? DocDate { get; set; }
    }
}