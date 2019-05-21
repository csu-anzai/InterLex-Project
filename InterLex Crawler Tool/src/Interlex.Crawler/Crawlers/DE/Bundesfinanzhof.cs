namespace Interlex.Crawler.Crawlers.DE
{
    using System;
    using System.Threading.Tasks;
    using log4net;

    /// <summary>
    /// Court: Bundesfinanzhofs
    /// MainUrl: https://juris.bundesfinanzhof.de/cgi-bin/rechtsprechung/list.py?Gericht=bfh&Art=en
    /// </summary>
    public class Bundesfinanzhof : BaseBundes
    {
        private const string BaseUrl = "https://juris.bundesfinanzhof.de/cgi-bin/rechtsprechung/list.py?Gericht=bfh&Art=en&Datum=";
        private const string MainUrl = "https://juris.bundesfinanzhof.de/cgi-bin/rechtsprechung/";

        public Bundesfinanzhof(ILog logger) : base(logger)
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
