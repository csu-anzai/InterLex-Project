namespace Interlex.Crawler.Crawlers.AT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using Interlex.Crawler.Common;
    using Interlex.Crawler.Model;
    using log4net;

    /// <summary>
    /// Represents crawler for the https://www.ris.bka.gv.at/ website
    /// </summary>
    public class RisBka : BaseHttpCrawler
    {
        private static readonly IReadOnlyCollection<String> initialPages = new HashSet<String>
        {
            "https://www.ris.bka.gv.at/Ergebnis.wxe?Abfrage=Vfgh&Entscheidungsart=Undefined&Sammlungsnummer=&Index=&SucheNachRechtssatz=True&SucheNachText=True&GZ=",
            "https://www.ris.bka.gv.at/Ergebnis.wxe?Abfrage=Vwgh&Entscheidungsart=Undefined&Sammlungsnummer=&Index=&AenderungenSeit=Undefined&SucheNachRechtssatz=True&SucheNachText=True&GZ=",
            "https://www.ris.bka.gv.at/Ergebnis.wxe?Abfrage=Justiz&Gericht=&Rechtssatznummer=&Rechtssatz=&Fundstelle=&AenderungenSeit=Undefined&SucheNachRechtssatz=True&SucheNachText=True&GZ=",
            "https://www.ris.bka.gv.at/Ergebnis.wxe?Abfrage=Bvwg&Entscheidungsart=Undefined&SucheNachRechtssatz=True&SucheNachText=True&GZ=",
            "https://www.ris.bka.gv.at/Ergebnis.wxe?Abfrage=Lvwg&Entscheidungsart=Undefined&Bundesland=Undefined&SucheNachRechtssatz=True&SucheNachText=True&GZ=",
            "https://www.ris.bka.gv.at/Ergebnis.wxe?Abfrage=Dsk&Entscheidungsart=Undefined&Organ=Undefined&SucheNachRechtssatz=True&SucheNachText=True&GZ="
        };

        public RisBka(Http http, ILog logger) : base(http, initialPages, logger)
        {
        }

        public override NameModel CreatePackageDocumentName(HtmlNode anchor, HttpModel httpGet)
        {
            return NameModel.Create("content");
        }

        public override NameModel CreatePackageName(HtmlNode anchor, HttpModel httpGet)
        {
            // example: https://www.ris.bka.gv.at/Dokument.wxe?ResultFunctionToken=8df6bc29-1253-484d-a41d-5db518f80b23&Abfrage=Bvwg&Entscheidungsart=Undefined&SucheNachRechtssatz=True&SucheNachText=True&GZ=&VonDatum=01.01.2014&BisDatum=14.05.2019&Norm=&ImRisSeitVonDatum=&ImRisSeitBisDatum=&ImRisSeit=Undefined&ResultPageSize=100&Suchworte=&Dokumentnummer=BVWGT_20190502_W170_2208106_1_00

            var documentNumber = new Uri(anchor.Href()).GetQueryFragmentsMap()["Dokumentnummer"];

            return NameModel.Create(documentNumber);
        }

        public override string GetTwoLetterLanguage(HttpModel httpGet, HtmlNode anchor)
        {
            return "DE";
        }

        public override bool IsNotFound(HttpModel httpGet)
        {
            return httpGet.StatusCode == System.Net.HttpStatusCode.NotFound;
        }

        public override bool IsPackage(HtmlNode anchor)
        {
            return anchor.HasClass("nonWrappingCell") && anchor.Href().Contains("&Dokumentnummer=");
        }

        public override bool IsPackageDocument(HtmlNode anchor)
        {
            return false;
        }

        public override bool ShouldExamine(HtmlNode anchor)
        {
            // example: /Ergebnis.wxe?Abfrage=Vfgh&Entscheidungsart=Undefined&Sammlungsnummer=&Index=&SucheNachRechtssatz=True&SucheNachText=True&GZ=&VonDatum=&BisDatum=14.05.2019&Norm=&ImRisSeitVonDatum=&ImRisSeitBisDatum=&ImRisSeit=Undefined&ResultPageSize=100&Suchworte=&Position=101

            var isPage = anchor.Href().Contains("ResultPageSize=") && anchor.Href().Contains("Position=");
            isPage = isPage && int.TryParse(anchor.InnerText.Trim(), out _);

            return isPage;
        }
    }
}
