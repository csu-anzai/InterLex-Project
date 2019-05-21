namespace Interlex.Crawler.Crawlers.DE
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    /// <summary>
    /// Court: Bundesarbeitsgericht
    /// MainUrl: https://juris.bundesarbeitsgericht.de/cgi-bin/rechtsprechung/list.py?Gericht=bag&Art=en
    /// </summary>
    public class Bundesarbeitsgericht : BaseBundes
    {
        private const string BaseUrl = "https://juris.bundesarbeitsgericht.de/cgi-bin/rechtsprechung/list.py?Gericht=bag&Art=en&Datum=";
        private const string MainUrl = "https://juris.bundesarbeitsgericht.de/cgi-bin/rechtsprechung/";

        public Bundesarbeitsgericht(ILog logger) : base(logger)
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
