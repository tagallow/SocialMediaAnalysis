using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SocialMonitorCloud
{
    public static class DatabaseSchemaUtility
    {
        public static void InitializeSchema()
        {
            CreateDatabase(ServerConfiguration.GetConnectionString(), "SocialMediaData_Dev");

            // tables
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.Tables.Accounts.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.Tables.DailyRecords.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.Tables.DataSourceIDs.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.Tables.KeywordAssignments.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.Tables.Keywords.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.Tables.Settings.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.Tables.SocialMediaData.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.Tables.ErrorLogs.sql");

            // stored procedures
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetAllStatistics.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.AssignKeyword.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.CollectTotals.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.CreateAccount.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.DisableAccount.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.DisableKeyword.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.EnableAccount.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.EnableKeyword.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GenerateData.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetAccount.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetAccounts.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetKeywords.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetTotalCount.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.InsertAddress.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.InsertSentiment.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.MinuteMood.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.MinuteVolume.sql");
            //ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.NegativeCountRange.sql");
            //ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.PositiveCountRange.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.ToggleAnalysis.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.ToggleKeyword.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.UnassignKeyword.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetUserKeywords.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetLocalUserID.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetKeywordInfo.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetActiveKeywords.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetLatestTwitterID.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.SetLatestTwitterID.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetAnnotations.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.InsertAnnotation.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetTimeRangeCounts.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.GetHistoryPage.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.DeleteOldData.sql");
            ExecuteSqlScript(ServerConfiguration.GetConnectionString(), "SocialMonitorCloud.Schema.StoredProcedures.InsertErrorLog.sql");
        }

        private static void CreateDatabase(string connectionString, string databaseName)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            connectionStringBuilder.InitialCatalog = "master";
            connectionString = connectionStringBuilder.ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM sys.databases WHERE name = @DatabaseName";
                command.Parameters.AddWithValue("@DatabaseName", databaseName);
                bool databaseExists = false;
                using (SqlDataReader reader = command.ExecuteReader())
                    databaseExists = reader.Read();

                if (!databaseExists)
                {
                    command = connection.CreateCommand();
                    command.CommandText = string.Format(@"CREATE DATABASE [{0}]", databaseName);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void ExecuteSqlScript(string connectionString, string embeddedResourceName)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            using (Stream stream = executingAssembly.GetManifestResourceStream(embeddedResourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContents = reader.ReadToEnd() + Environment.NewLine;
                    string[] sqlBlocks = fileContents.Split(new string[] { "GO\n", "go\n", "Go\n", "GO\r\n", "go\r\n", "Go\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
                    connectionStringBuilder.ConnectTimeout = 120;
                    using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ToString()))
                    {
                        connection.Open();
                        foreach (string sqlBlock in sqlBlocks)
                        {
                            SqlCommand command = connection.CreateCommand();
                            command.CommandText = sqlBlock;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}