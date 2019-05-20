using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.ResponseModels
{
    public class SuggestionsModel
    {
        public IEnumerable<string> Sources { get; set; }

        public IEnumerable<string> Courts { get; set; }

        public IEnumerable<string> CourtsEng { get; set; }

        public IEnumerable<string> Keywords { get; set; }
    }
}
