using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class CaseModel
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public string Title { get; set; }

        //[JsonRequired]
        //public DateTime Date { get; set; }
    }
}
