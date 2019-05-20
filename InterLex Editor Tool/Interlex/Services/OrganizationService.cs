using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Services
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Models.ResponseModels;

    public class OrganizationService
    {
        private readonly AppDbContext context;

        public OrganizationService(AppDbContext context)
        {
            this.context = context;
        }

        public Task<List<OrganizationModel>> GetOrganizationNames()
        {
            return this.context.Organizations.AsNoTracking().Select(x => new OrganizationModel
            {
                ShortName = x.ShortName,
                FullName = x.FullName
            }).ToListAsync();
        }
    }
}

