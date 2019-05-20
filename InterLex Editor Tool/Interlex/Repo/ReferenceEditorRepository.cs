namespace Interlex.Repo
{
    using System;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Dapper;
    using Interlex.Models.ResponseModels;

    public class ReferenceEditorRepository
    {
        private readonly String crawlerStorageConnectionString;

        public ReferenceEditorRepository(string crawlerStorageConnectionString)
        {
            this.crawlerStorageConnectionString = crawlerStorageConnectionString;
        }

        public async Task<ReferenceEditorActInfoModel> GetEuActInfoAsync(String celex)
        {
            using (var connection = new SqlConnection(this.crawlerStorageConnectionString))
            {
                var info = await connection.QueryFirstOrDefaultAsync<ReferenceEditorActInfoModel>(
                    $"select x.title as Title, x.ecli as Ecli, x.date as Date from CrawlerXml.interlex.f_get_meta_info_by_celex(@celex, @twoLetterLang) as x", 
                    new { @celex = celex, @twoLetterLang = "EN" }
                );

                return info;
            }
        }
    }
}
