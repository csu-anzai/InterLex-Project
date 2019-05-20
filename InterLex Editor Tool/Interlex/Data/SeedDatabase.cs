using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Interlex.Data
{
    using System.Security.Claims;
    using Common;
    using Interlex.Models.ResponseModels;
    using Microsoft.AspNetCore.Identity;

    public static class SeedDatabase
    {
        private static readonly string defaultOrg = "APIS";
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await context.Database.EnsureCreatedAsync();
            if (!context.Users.Any())
            {
                var user = new ApplicationUser
                {
                    Email = "aleksiev@apis.bg",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "aleksiev@apis.bg",
                    Organization = context.Organizations.FirstOrDefault(x => x.ShortName == defaultOrg)
                };
                await userManager.CreateAsync(user, "password123");
                await userManager.AddClaimsAsync(user, new[] { new Claim(Constants.Privileges, nameof(UserPrivileges.SuperAdmin)), new Claim(Constants.Organization, defaultOrg) });
                var user2 = new ApplicationUser
                {
                    Email = "notadmin@apis.bg",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "notadmin@apis.bg"
                };
                await userManager.CreateAsync(user2, "password123");
                await userManager.AddClaimsAsync(user2, new[] { new Claim(Constants.Privileges, nameof(UserPrivileges.User)), new Claim(Constants.Organization, defaultOrg) });
            }

            if (!context.Organizations.Any())
            {
                var orgs = new List<Organization>
                {
                    new Organization{Id = Guid.NewGuid().ToString(), ShortName = "APIS",   FullName = "Apis Europe"},
                    new Organization{Id = Guid.NewGuid().ToString(), ShortName = "UNITO", FullName = "Università degli Studi di Torino"},
                    new Organization{Id = Guid.NewGuid().ToString(), ShortName = "UNIBO", FullName = "Alma mater studiorum - Università di Bologna "},
                    new Organization { Id = Guid.NewGuid().ToString(), ShortName = "EUI", FullName = "European University Institute"},
                    new Organization{Id = Guid.NewGuid().ToString(), ShortName = "MU", FullName =  "Masarykova Univerzita"},
                    new Organization{Id = Guid.NewGuid().ToString(), ShortName =  "COA ROMA", FullName  = "Consiglio dell'Ordine degli Avvocati di Roma"},
                    new Organization {Id = Guid.NewGuid().ToString(), ShortName  ="UKON", FullName = "Universität Konstanz"}
                };
                await context.AddRangeAsync(orgs);
                await context.SaveChangesAsync();
            }
        }
    }
}