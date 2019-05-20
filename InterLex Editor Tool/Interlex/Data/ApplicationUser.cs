using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Data
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Cases = new HashSet<Case>();
            this.CasesLog = new HashSet<CaseLog>();
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }

        public ICollection<Case> Cases { get; set; }
        public ICollection<CaseLog> CasesLog { get; set; }

        public ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public string OrganizationId { get; set; }

        public Organization Organization { get; set; }

        public bool? IsActive { get; set; }
    }
}