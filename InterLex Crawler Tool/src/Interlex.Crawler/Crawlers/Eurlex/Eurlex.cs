namespace Interlex.Crawler.Crawlers.Eurlex
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HtmlAgilityPack;
    using Interlex.Crawler.Common;
    using Interlex.Crawler.Model;
    using log4net;

    /// <summary>
    /// Represents crawler for eurlex documents.
    /// Legislation for the directory legislation classification with code 19.20 (Judicial cooperation in civil matters) + consolidated versions
    /// Caselaw for the directory caselaw classification with code 4.06.02 (Judicial cooperation in civil matters)
    /// Jure documents.
    /// </summary>
    public class Eurlex : BaseHttpCrawler
    {
        private static readonly HashSet<String> allowedLanguages = new HashSet<string> { "BG", "DE", "FR", "EN", "IT" };

        public Eurlex(Http http, ILog logger) : base(
            http,
            new string[]
            {
                "https://eur-lex.europa.eu/search.html?qid=1557822576221&type=named&CC_1_CODED=19&name=browse-by:legislation-in-force&CC_2_CODED=1920", // legislation in force
                "https://eur-lex.europa.eu/search.html?qid=1557822576221&type=named&CC_1_CODED=19&name=browse-by:legislation-not-in-force&CC_2_CODED=1920", // legislation not in force
                "https://eur-lex.europa.eu/search.html?qid=1557825935236&RJ_NEW_2_CODED=4.06&RJ_NEW_1_CODED=4&type=named&RJ_NEW_3_CODED=4.06.02&name=browse-by:new-case-law", // caselaw
                "https://eur-lex.europa.eu/search.html?qid=1557834089988&CASE_LAW_SUMMARY=false&type=advanced&CASE_LAW_JURE_SUMMARY=false&lang=en&SUBDOM_INIT=JURE&DTS_SUBDOM=JURE", // jure
            },
            logger)
        {

        }

        public override NameModel CreatePackageDocumentName(HtmlNode anchor, HttpModel httpGet)
        {
            return NameModel.Create("content");

        }

        public override NameModel CreatePackageName(HtmlNode anchor, HttpModel httpGet)
        {
            // example: https://eur-lex.europa.eu/legal-content/BG/TXT/HTML/?uri=CELEX:32015R0848&qid=1557822576221&from=EN

            var twoLetterLang = this.GetTwoLetterLanguage(httpGet, anchor);
            if (TryGetCelexFromUrl(anchor.Href()) is String celex)
            {
                if (celex.StartsWith("8"))
                {
                    return NameModel.Create(celex.Replace("(", "_").Replace(")", String.Empty));
                }
                else
                {
                    return NameModel.Create($"{celex}_{twoLetterLang}");
                }
            }
            else
            {
                throw new ArgumentException($"Could not package name from: {anchor.Href()}");
            }
        }

        public override string GetTwoLetterLanguage(HttpModel httpGet, HtmlNode anchor)
        {
            // example: https://eur-lex.europa.eu/legal-content/BG/TXT/HTML/?uri=CELEX:32015R0848&qid=1557822576221&from=EN
            var twoLetterLang = anchor.Href()
                .Split(new String[] { "legal-content" }, StringSplitOptions.RemoveEmptyEntries)
                [1]
                .Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
                [0];

            var celex = TryGetCelexFromUrl(anchor.Href());

            // sector 8 languages could be 25 in number
            if (celex.StartsWith("8") == false && twoLetterLang.IsIn(allowedLanguages) == false)
            {
                throw new ArgumentException($"Not allowed language for download");
            }

            return twoLetterLang;
        }

        public override bool IsNotFound(HttpModel httpGet)
        {
            return httpGet.StatusCode == System.Net.HttpStatusCode.NotFound;
        }

        public override bool IsPackage(HtmlNode anchor)
        {
            var isPackage = false;

            if (TryGetCelexFromUrl(anchor.Href()) is String celex)
            {
                var allowedLangAndFormatIds = allowedLanguages.Select(x => $"format_language_table_HTML_{x}").ToList();

                isPackage = anchor.Id.IsIn(allowedLangAndFormatIds);
                isPackage = isPackage && anchor.GetClasses().Contains("disabled") == false;

                // for sector 8
                isPackage = isPackage || (celex.StartsWith("8") && anchor.Id.StartsWith("format_language_table_PDF") && anchor.GetClasses().Contains("disabled") == false);
            }

            return isPackage;
        }

        public override bool IsPackageDocument(HtmlNode anchor)
        {
            return false;
        }

        public override bool ShouldExamine(HtmlNode anchor)
        {
            var shouldExamine = anchor.Id.StartsWith("cellar_");
            shouldExamine = shouldExamine || anchor.ParentNode.Id == "pagingForm" && anchor.GetAttributeValue("title", String.Empty) == "Next Page";
            shouldExamine = shouldExamine && anchor.Href().Contains("print=true") == false;

            return shouldExamine;
        }

        public override IReadOnlyCollection<string> GenerateAdditionalUrls(HtmlNode anchor, HttpModel httpModel)
        {
            // generate consolidated version links

            var result = new List<String>();

            if (this.IsPackage(anchor))
            {
                var celex = TryGetCelexFromUrl(anchor.Href());
                if (celex.StartsWithAny("2", "3", "4")) // legislation
                {
                    // example: https://eur-lex.europa.eu/search.html?text=02015R0848-&scope=EURLEX&type=quick&lang=en

                    var consSearchNumber = celex
                        .SafeReplaceAtStart("2", "0")
                        .SafeReplaceAtStart("3", "0")
                        .SafeReplaceAtStart("4", "0");

                    result.Add($"https://eur-lex.europa.eu/search.html?text={consSearchNumber}-&scope=EURLEX&type=quick&lang=en");
                }
            }

            return result;
        }

        private static String TryGetCelexFromUrl(String url)
        {
            var celex = (String)null;

            // example: https://eur-lex.europa.eu/legal-content/BG/TXT/HTML/?uri=CELEX:32015R0848&qid=1557822576221&from=EN
            if (new Uri(url).GetQueryFragmentsMap().TryGetValue("uri", out var celexAnnotation))
            {
                if (celexAnnotation.StartsWith("CELEX:"))
                {
                    celex = celexAnnotation.SafeReplaceAtStart("CELEX:", String.Empty);
                }
            }

            return celex;
        }
    }
}
