namespace Interlex.Crawler.Crawlers.DE
{
    using log4net;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Court: Bundesgerichtshof
    /// MainUrl: https://juris.bundesgerichtshof.de/cgi-bin/rechtsprechung/list.py?Gericht=bgh&Art=en
    /// </summary>
    public class Bundesgerichtshof : BaseBundes
    {
        private const string BaseUrl = "https://juris.bundesgerichtshof.de/cgi-bin/rechtsprechung/list.py?Gericht=bgh&Art=en&Datum=";
        private const string MainUrl = "https://juris.bundesgerichtshof.de/cgi-bin/rechtsprechung/";

        public Bundesgerichtshof(ILog logger) : base(logger)
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
