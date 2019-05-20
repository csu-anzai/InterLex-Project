using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Data
{
    public class Source
    {
        public string Name { get; set; }
        public int JurId { get; set; }

        public Jurisdiction Jurisdiction { get; set; }
    }
}
