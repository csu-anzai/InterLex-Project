using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Repo
{
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class Repository
    {
        private readonly AppDbContext appDbContext;
        public Repository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public IEnumerable<string> GetAllLanguages()
        {
            var result = this.appDbContext.Languages.Select(x => x.Name).ToList();
            return result;
        }
    }
}
