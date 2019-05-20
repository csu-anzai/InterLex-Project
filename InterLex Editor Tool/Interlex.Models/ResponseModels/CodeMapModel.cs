using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.ResponseModels
{
    public class CodeMapModel
    {
        public bool Leaf { get; set; }
        public string Code { get; set; }
        public Guid Data { get; set; }
        public bool Selectable { get; set; }
        public string Label { get; set; }
    }
}
