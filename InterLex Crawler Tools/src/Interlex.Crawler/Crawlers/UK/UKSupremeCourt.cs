namespace Interlex.Crawler.Crawlers.UK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using Interlex.Crawler.Common;
    using Interlex.Crawler.Model;
    using log4net;

    /// <summary>
    /// Represents crawler for the https://www.supremecourt.uk/decided-cases/index.html website
    /// </summary>
    public class UKSupremeCourt : BaseHttpCrawler
    {
        public UKSupremeCourt(Http http, ILog logger) : base(http, "https://www.supremecourt.uk/decided-cases/index.html", logger)
        {
        }

        public override bool IsPackage(HtmlNode anchor)
        {
            var isPackage = IsJudgmentPdf(anchor);

            return isPackage;
        }

        public override bool IsPackageDocument(HtmlNode anchor)
        {
            var isPackageDocument = IsPressSummaryPdf(anchor);

            return isPackageDocument;
        }

        public override bool ShouldExamine(HtmlNode anchor)
        {
            // example: 2012.html
            var shouldExamine = Regex.Match(input: anchor.Href(), pattern: @"\d{4}\.html").Success;
            shouldExamine = shouldExamine || IsCasePage(anchor);

            return shouldExamine;
        }

        public override bool ShouldIncludeParentPageAsDocument(HtmlNode parentPageAnchor, HtmlNode currentAnchor)
        {
            return IsCasePage(parentPageAnchor);
        }

        public override NameModel CreatePackageDocumentName(HtmlNode anchor, HttpModel httpGet)
        {
            if (IsCasePage(anchor))
            {
                return NameModel.Create("detail");
            }
            else if (IsJudgmentPdf(anchor))
            {
                return NameModel.Create("content");
            }
            else if (IsPressSummaryPdf(anchor))
            {
                return NameModel.Create("presssummary");
            }
            else
            {
                throw new ArgumentException($"Unknown anchor pattern for package document name. Url {anchor.Href()}");
            }
        }

        public override NameModel CreatePackageName(HtmlNode anchor, HttpModel httpGet)
        {
            return NameModel.UnifyAndCreateFromUrlLastPart(httpGet.Url);
        }

        public override bool IsNotFound(HttpModel httpGet) => httpGet.StatusCode == System.Net.HttpStatusCode.NotFound;

        public override string GetTwoLetterLanguage(HttpModel httpGet, HtmlNode anchor)
        {
            return "EN";
        }


        private static bool IsJudgmentPdf(HtmlNode anchor)
        {
            return anchor.Href().Contains("/docs/uksc-") && anchor.GetAttributeValue("title", String.Empty) == "Judgment (PDF)";
        }

        private static bool IsPressSummaryPdf(HtmlNode anchor)
        {
            return anchor.Href().Contains("/docs/uksc-") && anchor.GetAttributeValue("title", String.Empty) == "Press summary (PDF)";
        }

        private static bool IsCasePage(HtmlNode anchor)
        {
            return anchor.Href().Contains("/cases/uksc-");
        }
    }
}
