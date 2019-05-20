using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Data
{
    public class Organization
    {
        public Organization()
        {
            Users = new HashSet<ApplicationUser>();
        }

        public string Id { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
    }
}
