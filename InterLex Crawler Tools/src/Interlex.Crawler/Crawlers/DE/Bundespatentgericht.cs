namespace Interlex.Crawler.Crawlers.DE
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    /// <summary>
    /// Court: Bundespatentgericht
    /// MainUrl: https://juris.bundespatentgericht.de/cgi-bin/rechtsprechung/list.py?Gericht=bpatg&Art=en
    /// </summary>
    public class Bundespatentgericht : BaseBundes
    {
        private const string BaseUrl = "https://juris.bundespatentgericht.de/cgi-bin/rechtsprechung/list.py?Gericht=bpatg&Art=en&Datum=";
        private const string MainUrl = "https://juris.bundespatentgericht.de/cgi-bin/rechtsprechung/";

        public Bundespatentgericht(ILog logger) : base(logger)
        {
        }

        public override async Task StartAsync()
        {
            var fromYear = 2000;
            var toYear = DateTime.UtcNow.Year;
            var crawlerName = this.GetType().FullName;

            await this.LoadAsync(BaseUrl, MainUrl, fromYear, toYear, crawlerName);
        }
    }
}
