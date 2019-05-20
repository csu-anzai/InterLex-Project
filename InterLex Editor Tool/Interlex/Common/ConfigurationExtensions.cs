using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Interlex.Common
{
    public static class ConfigurationExtensions
    {
        public static String GetCrawlerStorageConnection(this IConfiguration config)
        {
            return config.GetConnectionString("CrawlerStorageConnection");
        }
    }
}
