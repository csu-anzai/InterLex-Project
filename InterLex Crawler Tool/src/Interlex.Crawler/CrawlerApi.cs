using System.Diagnostics;
using HtmlAgilityPack;

[assembly: DebuggerDisplay("{Name}; {GetAttributeValue(\"href\", \"\")}", Target = typeof(HtmlNode))]

namespace Interlex.Crawler
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Interlex.Crawler.Common;
    using Interlex.Crawler.Crawlers;
    using Interlex.Crawler.Crawlers.DE;
    using Interlex.Crawler.Crawlers.Eurlex;
    using Interlex.Crawler.Crawlers.FR;
    using Interlex.Crawler.Crawlers.UK;
    using Interlex.Crawler.Manager;
    using Interlex.Crawler.Model;
    using log4net;

    /// <summary>
    /// Entry point for starting different crawlers
    /// </summary>
    public static class CrawlerApi
    {
        /// <summary>
        /// Starts crawler from the specified cmd arguments
        /// </summary>
        /// <param name="args">
        /// Atleast on argument which must represent the name of the crawler to be started (example: Eurlex or Interlex.Crawler.Crawlers.Eurlex.Eurlex).
        /// Allowed argumnets are: -retrycount, -timeout, -reusecookies, -allowredirect
        /// </param>
        /// <param name="logger">Logger</param>
        /// <returns></returns>
        public static async Task CreateCmdArgs(string[] args, ILog logger)
        {
            var crawler = CreateCrawlerFromCmdArgs(args, logger);
            if (crawler is BaseHttpCrawler bhc)
            {
                var manager = new HttpCrawlerManager(logger, CrawlerHttpManagerConfig.Default);
                await manager.StartAsync(bhc);
            }
            else
            {
                await (crawler as BaseGenericCrawler).StartAsync();
            }
        }

        /// <summary>
        /// Starts the <see cref="Crawlers.UK.UKSupremeCourt"/> crawler
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task RunUKSupremeCourtAsync(ILog logger)
        {
            var http = new Http();
            var crawler = new UKSupremeCourt(http, logger);
            var manager = new HttpCrawlerManager(logger, CrawlerHttpManagerConfig.Default);

            await manager.StartAsync(crawler);
        }

        /// <summary>
        /// Starts the <see cref="Crawlers.DE.Bundesarbeitsgericht"/> crawler
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task RunBundesarbeitsgerichtAsync(ILog logger)
        {
            var crawler = new Bundesarbeitsgericht(logger);

            await crawler.StartAsync();
        }

        /// <summary>
        /// Starts the <see cref="Crawlers.DE.Bundesfinanzhof"/> crawler
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task RunBundesfinanzhofsAsync(ILog logger)
        {
            var crawler = new Bundesfinanzhof(logger);

            await crawler.StartAsync();
        }

        /// <summary>
        /// Starts the <see cref="Crawlers.DE.Bundesgerichtshof"/> crawler
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task RunBundesgerichtshofAsync(ILog logger)
        {
            var crawler = new Bundesgerichtshof(logger);

            await crawler.StartAsync();
        }

        /// <summary>
        /// Starts the <see cref="Crawlers.DE.Bundespatentgericht"/> crawler
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task RunBundespatentgerichtAsync(ILog logger)
        {
            var crawler = new Bundespatentgericht(logger);

            await crawler.StartAsync();
        }

        /// <summary>
        /// Starts the <see cref="Crawlers.DE.Bundessozialgericht"/> crawler
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task RunBundessozialgerichtAsync(ILog logger)
        {
            var crawler = new Bundessozialgericht(logger);

            await crawler.StartAsync();
        }

        /// <summary>
        /// Starts the <see cref="Crawlers.Eurlex.Eurlex"/> crawler
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task RunEurlexAsync(ILog logger)
        {
            var http = new Http(Http.DefaultRetryCount, Http.DefaultReUseCookies, allowRedirect: true, Http.DefaultTimeout);
            var crawler = new Eurlex(http, logger);
            var manager = new HttpCrawlerManager(logger, CrawlerHttpManagerConfig.Default);

            await manager.StartAsync(crawler);
        }

        /// <summary>
        /// Stats the <see cref="Crawlers.FR.LegiFrance"/> crawler
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task RunLegifrance(ILog logger)
        {
            var crawler = new LegiFrance(logger);

            await crawler.StartAsync();
        }


        private static Object CreateCrawlerFromCmdArgs(String[] args, ILog logger)
        {
            if (args.Length >= 1)
            {
                // required
                var crawlerName = args[0];

                var crawlerType = (from a in AppDomain.CurrentDomain.GetAssemblies()
                                   from t in a.GetTypes()
                                   where t.BaseType == typeof(BaseHttpCrawler) || t.BaseType == typeof(BaseGenericCrawler)
                                   where crawlerName == t.Name || crawlerName == t.FullName
                                   select t).Single();

                var crawler = (object)null;

                if (crawlerType.BaseType == typeof(BaseHttpCrawler))
                {
                    // optional
                    var retryCount = Http.DefaultRetryCount;
                    if (GetArgument("-retrycount=") is String rc)
                    {
                        retryCount = int.Parse(rc);
                    }

                    var timeoutInMs = Http.DefaultTimeout;
                    if (GetArgument("-timeout=") is String to)
                    {
                        timeoutInMs = TimeSpan.FromMilliseconds(int.Parse(to));
                    }

                    var reuseCookies = Http.DefaultReUseCookies;
                    if (GetArgument("-reusecookies=") is String ruc)
                    {
                        reuseCookies = bool.Parse(ruc);
                    }

                    var allowRedirect = Http.DefaultAllowRedirect;
                    if (GetArgument("-allowredirect=") is String ar)
                    {
                        allowRedirect = bool.Parse(ar);
                    }

                    var http = new Http(retryCount, reuseCookies, allowRedirect, timeoutInMs);
                    crawler = (BaseHttpCrawler)Activator.CreateInstance(crawlerType, http, logger);
                }
                else if (crawlerType.BaseType == typeof(BaseGenericCrawler))
                {
                    crawler = (BaseGenericCrawler)Activator.CreateInstance(crawlerType, logger);
                }

                return crawler;

                String GetArgument(String name) => args.SingleOrDefault(x => x.StartsWith(name, StringComparison.OrdinalIgnoreCase))?.SafeReplaceAtStart(name, String.Empty).Trim();
            }
            else
            {
                return null;
            }
        }
    }
}
