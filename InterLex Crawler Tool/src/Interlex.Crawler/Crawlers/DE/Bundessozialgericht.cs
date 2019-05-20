namespace Interlex.Crawler.Crawlers.DE
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    /// <summary>
    /// Court: Bundessozialgericht
    /// MainUrl: https://juris.bundessozialgericht.de/cgi-bin/rechtsprechung/list.py?Gericht=bsg&Art=en
    /// </summary>
    public class Bundessozialgericht : BaseBundes
    {
        private const string BaseUrl = "https://juris.bundessozialgericht.de/cgi-bin/rechtsprechung/list.py?Gericht=bsg&Art=en&Datum=";
        private const string MainUrl = "https://juris.bundessozialgericht.de/cgi-bin/rechtsprechung/";

        public Bundessozialgericht(ILog logger) : base(logger)
        {
        }

        public override async Task StartAsync()
        {
            var fromYear = 2010;
            var toYear = DateTime.UtcNow.Year;
            var crawlerName = this.GetType().FullName;

            await this.LoadAsync(BaseUrl, MainUrl, fromYear, toYear, crawlerName);
        }
    }
}
