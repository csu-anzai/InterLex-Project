namespace Interlex.Crawler.ConsoleClient
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using log4net.Config;

    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = ConfigLogger();

            if (args.Length >= 1)
            {
                await CrawlerApi.CreateCmdArgs(args, logger);
            }
            else // for non cmd approach - call the crawler from the api you need to execute
            {
                await CrawlerApi.RunBundesarbeitsgerichtAsync(logger);
            }
        }

        private static ILog ConfigLogger()
        {
            var logRepositoryAssembly = Assembly.GetEntryAssembly();
            var logRepository = LogManager.GetRepository(logRepositoryAssembly);
            var configFileInfo = new FileInfo("log4net.config");
            XmlConfigurator.Configure(logRepository, configFileInfo);
            var logger = LogManager.GetLogger(logRepositoryAssembly, "RollingFileAppender");

            return logger;
        }
    }
}
