namespace Interlex.Crawler.Data
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// EF model for the InterlexCrawler database
    /// </summary>
    public class InterlexCrawlerEntities : DbContext
    {
        /// <summary>
        /// EF dbset for the Crawlers table
        /// </summary>
        public DbSet<Crawler> Crawlers { get; set; }

        /// <summary>
        /// EF dbset for the Documents table
        /// </summary>
        public DbSet<Document> Documents { get; set; }

        /// <summary>
        /// EF dbset for the DocumentGroups table
        /// </summary>
        public DbSet<DocumentGroup> DocumentGroups { get; set; }

        /// <summary>
        /// EF dbset for the OperationStatuses table
        /// </summary>
        public DbSet<OperationStatus> OperationStatuses { get; set; }

        /// <summary>
        /// EF dbset for the OperationStatusLogs
        /// </summary>
        public DbSet<OperationStatusLog> OperationStatusLogs { get; set; }

        /// <summary>
        /// Procedure used to change the operation status for specific document gorup identifier
        /// </summary>
        /// <param name="identifier">Document group identifier</param>
        /// <param name="operation">New operation status id</param>
        /// <param name="caller">Caller of the function. Example: 'Interlex.Crawlers'</param>
        /// <param name="errorMessage">Error message if the <paramref name="operation"/> is error status code</param>
        /// <returns></returns>
        public int PChangeOperationStatus(String identifier, int operation, String caller, String errorMessage)
        {
            var identifierParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@Identifier", SqlDbType = System.Data.SqlDbType.UniqueIdentifier, Direction = System.Data.ParameterDirection.Input, Value = Guid.Parse(identifier) };

            var newStatusParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@NewStatus", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Input, Value = operation, Precision = 10, Scale = 0 };

            var authenticatorParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@authenticator", SqlDbType = System.Data.SqlDbType.NVarChar, Direction = System.Data.ParameterDirection.Input, Value = caller, Size = 250 };
            if (authenticatorParam.Value == null)
                authenticatorParam.Value = System.DBNull.Value;

            var errorMessageParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@errorMessage", SqlDbType = System.Data.SqlDbType.NVarChar, Direction = System.Data.ParameterDirection.Input, Value = errorMessage, Size = -1 };
            if (errorMessageParam.Value == null)
                errorMessageParam.Value = System.DBNull.Value;

            var procResultParam = new System.Data.SqlClient.SqlParameter { ParameterName = "@procResult", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output };

            this.Database.ExecuteSqlCommand("EXEC @procResult = [dbo].[p_ChangeOperationStatus] @Identifier, @NewStatus, @authenticator, @errorMessage", identifierParam, newStatusParam, authenticatorParam, errorMessageParam, procResultParam);

            return (int)procResultParam.Value;
        }

        /// <summary>
        /// Returns information for the new or changed documents in the interlex editor tool module
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(String id, String json)> GetNewOrUpdatedInterlexEditorToolLazy()
        {
            var connection = this.Database.GetDbConnection();
            var command = connection.CreateCommand();
            command.CommandText = "select x.id, x.content from f_get_new_or_update_interlex_editor_tool_docs() as x";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = (String)reader["id"];
                    var content = (String)reader["content"];

                    yield return (id, content);
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionString = ConfigurationManager.ConnectionStrings["CrawlerInterlex"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Crawler>().ToTable("Crawlers", "dbo").HasKey(x => x.CrawlerId);
            modelBuilder.Entity<Document>().ToTable("Documents", "dbo").HasKey(x => x.DocumentId);
            modelBuilder.Entity<DocumentGroup>().ToTable("DocumentGroups", "dbo").HasKey(x => x.DocumentGroupId);
            modelBuilder.Entity<DocumentGroup>()
                .HasMany(x => x.Documents)
                .WithOne(x => x.DocumentGroup)
                .HasPrincipalKey(x => x.DocumentGroupId)
                .HasForeignKey(x => x.DocumentGroupId);

            modelBuilder.Entity<OperationStatusLog>().ToTable("OperationStatusLogs", "dbo").HasKey(x => x.Id);
            modelBuilder.Entity<OperationStatus>().ToTable("OperationStatus", "dbo").HasKey(x => x.Identifier);
            modelBuilder.Entity<OperationStatus>()
                .HasMany(x => x.OperationStatusLogs)
                .WithOne(x => x.OperationStatus)
                .HasPrincipalKey(x => x.Identifier)
                .HasForeignKey(x => x.OperationStatusIdentifier);
        }

        /// <summary>
        /// Entity for the Crawlers table
        /// </summary>
        public class Crawler
        {
            public int CrawlerId { get; set; }
            public String CrawlerName { get; set; }
        }

        /// <summary>
        /// Entity for the Documents table
        /// </summary>
        public class Document
        {
            public int DocumentId { get; set; }
            public String DocumentFormat { get; set; }
            public String DocumentName { get; set; }
            public String Identifier { get; set; }
            public int Operation { get; set; }
            public String Url { get; set; }
            public String Md5 { get; set; }
            public int DocumentOrder { get; set; }
            public int DocumentGroupId { get; set; }
            public virtual DocumentGroup DocumentGroup { get; set; }
        }

        /// <summary>
        /// Entity for the DocumentGroups table
        /// </summary>
        public class DocumentGroup
        {
            public int DocumentGroupId { get; set; }
            public String DocumentGroupDate { get; set; }
            public String Lang { get; set; }
            public int GroupType { get; set; }
            public String DocumentGroupFormat { get; set; }
            public String DocumentGroupName { get; set; }
            public String Identifier { get; set; }
            public int Operation { get; set; }
            public Byte[] DataContent { get; set; }
            public int CrawlerId { get; set; }

            public virtual ICollection<Document> Documents { get; set; } = new HashSet<Document>();
        }

        /// <summary>
        /// Entity for the OperationStatus table
        /// </summary>
        public class OperationStatus
        {
            public Guid Identifier { get; set; }
            public int CurrentStatus { get; set; }
            public DateTime LastModificationDate { get; set; }
            public virtual ICollection<OperationStatusLog> OperationStatusLogs { get; set; } = new HashSet<OperationStatusLog>();
        }

        /// <summary>
        /// Entity for the OperationstatusLogs table
        /// </summary>
        public class OperationStatusLog
        {
            public int Id { get; set; }
            public DateTime LogDate { get; set; }
            public int? OldStatus { get; set; }
            public int? NewStatus { get; set; }
            public Guid OperationStatusIdentifier { get; set; }
            public virtual OperationStatus OperationStatus { get; set; }
            public String Authenticator { get; set; }
            public String ErrorMessage { get; set; }
        }
    }
}
