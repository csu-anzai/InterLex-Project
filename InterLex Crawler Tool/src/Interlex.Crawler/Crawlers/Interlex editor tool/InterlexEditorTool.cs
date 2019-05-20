namespace Interlex.Crawler.Crawlers.Interlex_editor_tool
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Text;
    using System.Threading.Tasks;
    using Interlex.Crawler.Data;
    using Interlex.Crawler.Model;
    using log4net;

    /// <summary>
    /// Represents crawler for the cases and legislation processed in the Interlex Editor Tool
    /// </summary>
    public class InterlexEditorTool : BaseGenericCrawler
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public InterlexEditorTool(ILog logger) : base(logger)
        {
        }

        public override Task StartAsync()
        {
            using (var context = new InterlexCrawlerEntities())
            {
                foreach (var (id, content) in context.GetNewOrUpdatedInterlexEditorToolLazy())
                {
                    try
                    {
                        var documentGroup = new DocumentGroupModel
                        {
                            CrawlerId = this.CrawlerId,
                            Name = id,
                            TwoLetterLanguage = "EU",
                            Documents =
                            {
                                new DocumentModel
                                {
                                    Raw = encoding.GetBytes(content),
                                    Name = "content",
                                    Format = "application/json",
                                    Url = "local"
                                }
                            }
                        };

                        this.DocumentGroupManager.AddOrUpdateDocumentGroup(documentGroup);
                    }
                    catch (Exception e)
                    {
                        this.Logger.Error($"{id}", e);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
