using SocialMonitorCloud.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Timers;

namespace SocialMonitorCloud
{
    public class DatabaseAccessLayer
    { 
        private static DatabaseAccessLayer _Instance;
        //private Timer _UpdateAverageTimer;

        public static DatabaseAccessLayer Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new DatabaseAccessLayer();
                return _Instance;
            }
        }

        protected DatabaseAccessLayer()
        {
            // TODO: Complete member initialization
        }
        public void LogError(Exception error)
        {
            if (error!=null)
            {
                LogError(error.Message, error.StackTrace);
            }
        }
        public void LogError(string errorStr,string stackTrace = "")
        {

            if (!string.IsNullOrEmpty(errorStr))
            {
                string connection_string = ServerConfiguration.GetConnectionString();

                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "[dbo].[InsertErrorLog]";
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@date", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@exception", errorStr);
                        command.Parameters.AddWithValue("@stackTrace", stackTrace);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public APIDataModel CollectRawData(string keyword)
        {
            APIDataModel d = new APIDataModel();
            List<int> pos = new List<int>();
            List<int> neg = new List<int>();
            List<string> dates = new List<string>();
            List<string> annotations = new List<string>();
            List<string> annotationTitles = new List<string>();

            string connection_string = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[GetAllStatistics]";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@keyword", keyword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pos.Add(int.Parse(reader["posCount"].ToString()));
                            neg.Add(int.Parse(reader["negCount"].ToString()));
                            dates.Add(DateTime.Parse(reader["dateCreated"].ToString()).ToShortDateString());
                            try
                            {
                                annotations.Add(reader["Annotation"].ToString());
                                annotationTitles.Add(reader["AnnotationTitle"].ToString());
                            }
                            catch 
                            {
                                annotations.Add(String.Empty);
                                annotationTitles.Add(String.Empty);
                            }
                        }
                    }
                }
            }
            d.posList = pos.ToArray();
            d.negList = neg.ToArray();
            d.dates = dates.ToArray();
            d.annotations = annotations.ToArray();
            d.annotationTitles = annotationTitles.ToArray();

            return d;
        }

        public int GetMinuteVolume(string keyword, TimeSpan upperBound, TimeSpan lowerBound)
        {
            int result = 0;
            string connection_string = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[MinuteVolume]";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@startTime", DateTime.Now.ToUniversalTime().Subtract(lowerBound));
                    command.Parameters.AddWithValue("@endTime", DateTime.Now.ToUniversalTime().Subtract(upperBound));
                    command.Parameters.AddWithValue("@keyword", keyword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        result = reader.GetInt32(0);
                    }
                }
                connection.Close();
            }
            return result;
        }
        public double GetMinuteMood(string keyword, TimeSpan upperBound, TimeSpan lowerBound)
        {
            int pos = 0;
            int neg = 0;
            double result = 0;
            string connection_string = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[MinuteMood]";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@startTime", DateTime.Now.ToUniversalTime().Subtract(lowerBound));
                    command.Parameters.AddWithValue("@endTime", DateTime.Now.ToUniversalTime().Subtract(upperBound));
                    command.Parameters.AddWithValue("@keyword", keyword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        pos = int.Parse(reader["posCount"].ToString());
                        neg = int.Parse(reader["negCount"].ToString());
                    }
                }
                connection.Close();
            }
            if (pos + neg > 0)
                result = ((double)pos - neg) / (pos + neg);

            return result;
        }
        public List<KeywordModel> GetKeywords()
        {
            List<KeywordModel> result = new List<KeywordModel>();
            KeywordModel temp;

            string connection_string = ServerConfiguration.GetConnectionString();
            //string sqlStatement = "select keyword, isActive, dateModified from keywords";

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[GetKeywords]";
                    command.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            temp = new KeywordModel();
                            temp.keyword = reader["keyword"].ToString();
                            temp.isActive = reader.GetBoolean(1);
                            temp.ID = reader.GetInt32(3);
                            temp.dateModified = DateTime.Parse(reader["dateModified"].ToString()).ToString();
                            result.Add(temp);
                        }
                    }
                }
                
            }
            return result;
        }
        public void EnableKeyword(string keyword)
        {
            string connection_string = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].EnableKeyword";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@keyword", keyword);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void DisableKeyword(string keyword)
        {
            string connection_string = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = "[dbo].DisableKeyword";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@keyword", keyword);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void CreateAccount(int UserID, string FirstName, string LastName)
        {
            string connection_string = ServerConfiguration.GetConnectionString();
            Guid guid = Guid.NewGuid();
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].CreateAccount";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@LocalAccountID", guid);
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.ExecuteNonQuery(); 
                }
                connection.Close();
            }
        }
        public void DeleteAccount(int UserID)
        {
            //WebSecurity.InitializeDatabaseConnection(
            //((SimpleMembershipProvider)Membership.Provider).DeleteAccount(name);
            //((SimpleMembershipProvider)Membership.Provider).DeleteUser(name, true);

            string connection_string = ServerConfiguration.GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "delete from Accounts where UserId=@id";
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@Name", UserID);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public List<Account> GetAccounts()
        {
            //List<string> keywords = new List<string>();
            List<Account> result = new List<Account>();
            Account temp;

            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[GetAccounts]";
                    command.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            temp = new Account();
                            temp.LocalAccountId = Guid.Parse(reader["LocalAccountID"].ToString());
                            temp.FirstName = reader["FirstName"].ToString();
                            //temp.LastName = reader["LastName"].ToString();
                            temp.IsActive = (bool)reader["IsActive"];
                            //temp.AccountId = int.Parse(reader["AccountID"].ToString());
                            temp.phoneNumber = reader["PhoneNum"].ToString();
                            temp.email = reader["email"].ToString();
                            temp.companyName = reader["companyName"].ToString();
                            temp.address1 = reader["address1"].ToString();
                            temp.address2 = reader["address2"].ToString();
                            temp.address3 = reader["address3"].ToString();
                            temp.city = reader["city"].ToString();
                            temp.state = reader["state"].ToString();
                            temp.zip = reader["zip"].ToString();
                            temp.getKeywords();
                            result.Add(temp);
                        }
                    }
                }
                connection.Close();
            }
            return result;
        }
        public Account GetAccountInfo(Guid id)
        {
            //List<string> keywords = new List<string>();

            Account result = new Account(id);
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "dbo.GetAccount";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        result.FirstName = reader["FirstName"].ToString();
                        //result.LastName = reader["LastName"].ToString();
                        result.IsActive = Boolean.Parse(reader["isActive"].ToString());
                        result.LocalAccountId = Guid.Parse(reader["LocalAccountID"].ToString());
                        result.phoneNumber = reader["PhoneNum"].ToString();
                        result.email = reader["email"].ToString();
                        result.companyName = reader["companyName"].ToString();
                        result.address1 = reader["address1"].ToString();
                        result.address2 = reader["address2"].ToString();
                        result.address3 = reader["address3"].ToString();
                        result.city = reader["city"].ToString();
                        result.state = reader["state"].ToString();
                        result.zip = reader["zip"].ToString();
                        result.getKeywords();
                    }
                }
                connection.Close();
            }
            result.getKeywords();
            return result;
        }
        public void UpdateAccountInfo(Account account)
        {
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "dbo.InsertAddress";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AccountID", account.LocalAccountId);
                    command.Parameters.AddWithValue("@phoneNum", account.phoneNumber);
                    command.Parameters.AddWithValue("@email", account.email);
                    command.Parameters.AddWithValue("@companyName", account.companyName);
                    command.Parameters.AddWithValue("@address1", account.address1);
                    command.Parameters.AddWithValue("@address2", account.address2);
                    command.Parameters.AddWithValue("@address3", account.address3);
                    command.Parameters.AddWithValue("@city", account.city);
                    command.Parameters.AddWithValue("@state", account.state);
                    command.Parameters.AddWithValue("@zip", account.zip);
                    //command.Parameters.AddWithValue("@country", account.country);
                    //command.Parameters.AddWithValue("@appartmentNum", account.apartmentNum);

                    command.ExecuteNonQuery(); 
                }
                connection.Close();
            }
        }
        public void DisableAccount(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].DisableAccount";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                    connection.Close(); 
                }
            }
        }
        public void EnableAccount(Guid id)
        {
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].EnableAccount";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public void UnAssignKeyword(Guid UserID, string keyword)
        {
            //Guid id = DatabaseAccessLayer.Instance.GetUserID(userName);
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "dbo.UnAssignKeyword";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AccountId", UserID);
                    command.Parameters.AddWithValue("@keyword", keyword);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AssignKeyword(Guid UserID, string keyword)
        {
            //Guid id = DatabaseAccessLayer.Instance.GetUserID(userName);
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "dbo.AssignKeyword";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@AccountId", UserID);
                    command.Parameters.AddWithValue("@keyword", keyword);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public List<int> GetUserKeywords(Guid UserID)
        {
            List<int> keywordIDs = new List<int>();
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[GetUserKeywords]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", UserID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            keywordIDs.Add(reader.GetInt32(0));
                        }
                    }
                }
                connection.Close();
            }
            

            return keywordIDs;
        }

        public KeywordModel GetKeywordFromID(int id)
        {
            KeywordModel result = new KeywordModel();
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[GetKeywordInfo]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        result.keyword = reader["keyword"].ToString();
                        result.isActive = reader.GetBoolean(1);
                        result.dateModified = reader[2].ToString();
                    }
                }
                connection.Close();
            }
            result.ID = id;
            result.chartDiv = result.keyword.Replace(" ", String.Empty) + "ChartDiv";
            return result;
        }
        public Guid GetLocalUserID(String name)
        {
            Guid result;
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[GetLocalUserID]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserName", name);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        result = Guid.Parse(reader[0].ToString());
                    }
                }
                connection.Close();
            }
            return result;
        }
        public bool AccountExists(int UserID)
        {
            bool result = false;
            int count;
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "select count(*) from Accounts where [UserID] = @id";
                    //command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", UserID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        count = reader.GetInt32(0);
                    }
                }
                connection.Close();
            }
            if (count == 1)
                result = true;

            return result;
        }
        public bool AccountExists(string userName)
        {
            bool result = false;
            int count;
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "select count(*) from Accounts where [FirstName] = @id";
                    //command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", userName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        count = reader.GetInt32(0);
                    }
                }
                connection.Close();
            }
            if (count == 1)
                result = true;

            return result;
        }
        public Guid GetLocalAccountID(int UserID)
        {
            Guid id;

            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "select [LocalAccountID] from Accounts where [UserID] = @id";
                    //command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", UserID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        id = Guid.Parse(reader["LocalAccountID"].ToString());
                    }
                }
                connection.Close();
            }

            return id;
        }
        public void InsertSentiment(Sentiment sent)
        {
            string connectionString = ServerConfiguration.GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                if (!string.IsNullOrEmpty(sent.text))
                {
                    if (sent.text.Length > 200)
                    {
                        sent.text = sent.text.Substring(0, 199);
                    }
                    if (sent.author.Length > 20)
                    {
                        sent.author = sent.author.Substring(0, 19);
                    }
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "[dbo].InsertSentiment";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@source", sent.source);
                    command.Parameters.AddWithValue("@datePosted", sent.time.ToUniversalTime());
                    command.Parameters.AddWithValue("@mood", sent.mood.ToLower());
                    command.Parameters.AddWithValue("@prob", sent.prob);
                    command.Parameters.AddWithValue("@text", sent.text);
                    command.Parameters.AddWithValue("@keyword", sent.keyword);
                    command.Parameters.AddWithValue("@author", sent.author);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void InsertAnnotation(string Keyword, string Title, string Caption, string Date)
        {
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].InsertAnnotation";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@keyword",Keyword);
                    command.Parameters.AddWithValue("@title",Title);
                    command.Parameters.AddWithValue("@caption",Caption);
                    command.Parameters.AddWithValue("@date",Date);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public bool IsActive(string keyword)
        {
            bool IsActive;

            string connection_string = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "select isActive from keywords where keyword=@k";
                    //command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@k", keyword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        IsActive = (bool)reader["IsActive"];
                    }
                }
                connection.Close();
            }
            return IsActive;
        }
        public APIDataModel GetAnnotations(string keyword)
        {
            List<int> AnnotationIDs = new List<int>();
            List<string> Annotations = new List<string>();
            List<string> AnnotationTitles = new List<string>();
            List<string> dates = new List<string>();
            APIDataModel d = new APIDataModel();
            d.keyword = keyword;
            string connection_string = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "dbo.GetAnnotations";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@keyword", keyword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            AnnotationIDs.Add((int)reader["ID"]);
                            AnnotationTitles.Add(reader["AnnotationTitle"].ToString());
                            Annotations.Add(reader["Annotation"].ToString());
                            dates.Add(DateTime.Parse(reader["dateCreated"].ToString()).ToShortDateString());
                        }
                    }
                }
                connection.Close();
            }
            d.AnnotationIDs = AnnotationIDs.ToArray();
            d.annotations = Annotations.ToArray();
            d.annotationTitles = AnnotationTitles.ToArray();
            d.dates = dates.ToArray();
            return d;
        }
        public int GetKeywordID(string keyword)
        {
            int id = -1;
            string connection_string = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandType = CommandType.Text;
                    command.CommandText = "select ID from Keywords where keyword = @keyword";

                    command.Parameters.AddWithValue("@keyword", keyword);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            id = (int)reader["ID"];
                    }
                }
                connection.Close();
            }
            return id;
        }
        public APIDataModel GetDayValues(string keyword, DateTime day)
        {
            APIDataModel data = new APIDataModel();

            data.keyword = keyword;
            int keywordID = GetKeywordID(keyword);
            string query = "select top 20 dateCreated,moodBit,text,author,sourceID from Social_Media_Data where keywordID = @keywordID and dateCreated > @day and dateCreated < @nextDay order by dateCreated desc";
            
            data.sentimentList = new List<Sentiment>();
            Sentiment sent;
            string connection_string = ServerConfiguration.GetConnectionString();
            int sourceid;
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandType = CommandType.Text;
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@keywordID", keywordID);
                    command.Parameters.AddWithValue("@day", day);
                    command.Parameters.AddWithValue("@nextDay", day.Add(TimeSpan.FromDays(1)));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sent = new Sentiment();
                            if((bool)reader["moodBit"])
                            {
                                sent.mood = "positive";
                            }
                            else
                            {
                                sent.mood = "negative";
                            }
                            sent.text = (string)reader["text"];
                            sent.author = (string)reader["author"];
                            sent.time = (DateTime)reader["dateCreated"];

                            sourceid = int.Parse(reader["sourceID"].ToString());
                            sent.sourceID = sourceid;
                            if (sourceid == 1)
                            {
                                sent.source = "Twitter";
                            }
                            else if (sourceid == 7)
                            {
                                sent.source = "GooglePlus";
                            }
                            else if (sourceid > 1 && sourceid < 7)
                            {
                                sent.source = "Facebook";
                            }
                            else
                            {
                                sent.source = "Unknown";
                            }

                            sent.timeString = sent.time.ToShortTimeString();
                            data.sentimentList.Add(sent);
                        }
                    }
                }
                connection.Close();
            }
            
            return data;
        }
        public APIDataModel GetHistoryPage(string keyword, int page, DateTime latestDate)
        {
            if (page < 0)
            {
                return null;
            }
            APIDataModel data = new APIDataModel();
            data.sentimentList = new List<Sentiment>();
            string connection_string = ServerConfiguration.GetConnectionString();
            Sentiment currentSentiment=null;
            int sourceid;

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dbo.GetHistoryPage";

                    command.Parameters.AddWithValue("@keyword", keyword);
                    command.Parameters.AddWithValue("@page", page);
                    command.Parameters.AddWithValue("@increment", 10);
                    command.Parameters.AddWithValue("@latestDate", latestDate);
                    

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            currentSentiment = new Sentiment();
                            currentSentiment.author = (string)reader["author"];
                            currentSentiment.time = (DateTime)reader["datePosted"];
                            currentSentiment.timeString = currentSentiment.time.ToLocalTime().ToShortTimeString();
                            currentSentiment.text = (string)reader["text"];
                            sourceid = int.Parse(reader["sourceID"].ToString());
                            currentSentiment.sourceID = sourceid;
                            if (sourceid == 1)
                            {
                                currentSentiment.source = "Twitter";
                            }
                            else if (sourceid == 7)
                            {
                                currentSentiment.source = "GooglePlus";
                            }
                            else if (sourceid > 1 && sourceid < 7)
                            {
                                currentSentiment.source = "Facebook";
                            }
                            else
                            {
                                currentSentiment.source = "Unknown";
                            }

                            data.sentimentList.Add(currentSentiment);
                        }
                        
                    }
                }
                connection.Close();
            }

            return data;
        }
        public void PruneData()
        {
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select top 100 moodbit,dateCreated,[text],author,sourceID from Social_Media_Data where [keywordID] = @keywordID and [dateCreated] >= @time";

                    //command.Parameters.AddWithValue("@keywordID", keywordID);
                    command.Parameters.AddWithValue("@date",DateTime.Now.Subtract(TimeSpan.FromDays(90)).ToUniversalTime());

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public APIDataModel GetTenSeconds(int keywordID,string source)
        {
            APIDataModel data = new APIDataModel();

            List<Sentiment> sentList = new List<Sentiment>();
            string connection_string = ServerConfiguration.GetConnectionString();
            
            List<string> timeString = new List<string> { DateTime.Now.ToShortTimeString() };
            data.posList = new int[1];
            data.negList = new int[1];
            DateTime time = DateTime.Now.Subtract(TimeSpan.FromSeconds(20)).ToUniversalTime();
            data.normalizedMood = new double[1];
            data.sentimentList = new List<Sentiment>();
            Sentiment currentSentiment = null;
            int sourceid;
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select top 100 moodbit,dateCreated,[text],author,sourceID from Social_Media_Data where [keywordID] = @keywordID and [dateCreated] >= @time";

                    command.Parameters.AddWithValue("@keywordID", keywordID);
                    command.Parameters.AddWithValue("@time",time);
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            currentSentiment = new Sentiment();
                            currentSentiment.author = (string)reader["author"];
                            currentSentiment.time = (DateTime)reader["dateCreated"];
                            currentSentiment.timeString = currentSentiment.time.ToShortTimeString();
                            currentSentiment.text = (string)reader["text"];
                            sourceid = int.Parse(reader["sourceID"].ToString());
                            currentSentiment.sourceID = sourceid;
                            if (sourceid == 1)
                            {
                                currentSentiment.source = "Twitter";
                            }
                            else if (sourceid == 7)
                            {
                                currentSentiment.source = "GooglePlus";
                            }
                            else if (sourceid > 1 && sourceid < 7)
                            {
                                currentSentiment.source = "Facebook";
                            }
                            else
                            {
                                currentSentiment.source = "Unknown";
                            }

                            if ((bool)reader["moodbit"])
                            {
                                data.posList[0]++;
                                currentSentiment.mood = "Positive";
                            }
                            else
                            {
                                data.negList[0]++;
                                currentSentiment.mood = "Negative";
                            }
                            if (string.IsNullOrEmpty(source) || source == currentSentiment.source)
                            {
                                data.sentimentList.Add(currentSentiment);
                            }
                        }
                        
                    }
                }
                connection.Close();
            }
            if (data.posList[0] + data.negList[0] > 0)
                data.normalizedMood[0] = ((double)data.posList[0] - data.negList[0]) / (data.posList[0] + data.negList[0]);
            else
                data.normalizedMood[0] = 0;

            data.dates = timeString.ToArray();
            return data;
        }
        public APIDataModel Get5Min(int keywordID)
        {
            APIDataModel data = new APIDataModel();
            
            data.sentimentList = new List<Sentiment>();
            string connection_string = ServerConfiguration.GetConnectionString();
            List<DateTime> times = new List<DateTime>();
            List<string> timeString = new List<string>();
            for (int i = 4; i >= 0; i--)
            {
                times.Add(DateTime.Now.Subtract(TimeSpan.FromMinutes(i)));
                timeString.Add(DateTime.Now.Subtract(TimeSpan.FromMinutes(i)).ToShortTimeString());
            }
            data.posList = new int[5];
            data.negList = new int[5];
            data.normalizedMood = new double[5];
            
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();
                for(int i=0;i<5;i++)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.GetTimeRangeCounts";

                        command.Parameters.AddWithValue("@keywordID", keywordID);
                        command.Parameters.AddWithValue("@startTime", times[i].ToUniversalTime());
                        command.Parameters.AddWithValue("@endTime", times[i].Add(TimeSpan.FromMinutes(1)).ToUniversalTime());
                        
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            data.posList[i] = (int)reader["Positive"];
                            data.negList[i] = (int)reader["Negative"];
                            if (data.posList[i] + data.negList[i] > 0)
                                data.normalizedMood[i] = ((double)data.posList[i] - data.negList[i]) / (data.posList[i] + data.negList[i]);
                            else
                                data.normalizedMood[i] = 0;
                        }
                    }
                }
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "select top 20 datePosted,moodBit,text,author,sourceID from Social_Media_Data where keywordID = @keywordID and datePosted > @time order by datePosted desc";

                    command.Parameters.AddWithValue("@keywordID", keywordID);
                    command.Parameters.AddWithValue("@time", DateTime.Now.Subtract(TimeSpan.FromMinutes(6)).ToUniversalTime());
                    Sentiment sent;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sent = new Sentiment();
                            sent.text = (string)reader["text"];
                            sent.author = (string)reader["author"];
                            sent.time = (DateTime)reader["datePosted"];

                            sent.sourceID = int.Parse(reader["sourceID"].ToString());
                            if (sent.sourceID == 1)
                            {
                                sent.source = "Twitter";
                            }
                            else if (sent.sourceID == 7)
                            {
                                sent.source = "GooglePlus";
                            }
                            else if (sent.sourceID > 1 && sent.sourceID < 7)
                            {
                                sent.source = "Facebook";
                            }
                            else
                            {
                                sent.source = "Unknown";
                            }

                            sent.timeString = sent.time.ToShortTimeString();
                            data.sentimentList.Add(sent);
                        }
                    }
                }
                connection.Close();
            }
            data.dates = timeString.ToArray();

            return data;
        }

        public APIDataModel GetTimeRange(int keywordID, string unit)
        {
            DateTime date = DateTime.Today;
            
            if (unit == "1week")
            {
                date = DateTime.Today.Subtract(TimeSpan.FromDays(7));
            }
            else if (unit == "2week")
            {
                date = DateTime.Today.Subtract(TimeSpan.FromDays(7*2));
            }
            else if (unit == "1month")
            {
                date = DateTime.Today.Subtract(TimeSpan.FromDays(30));
            }
            else if (unit == "3month")
            {
                date = DateTime.Today.Subtract(TimeSpan.FromDays(30*3));
            }
            else if (unit == "6month")
            {
                date = DateTime.Today.Subtract(TimeSpan.FromDays(30*6));
            }
            else if (unit == "1year")
            {
                date = DateTime.Today.Subtract(TimeSpan.FromDays(365));
            }
            else if (unit == "5min")
            {
                date = DateTime.Now.Subtract(TimeSpan.FromMinutes(6));
            }

            List<int> positive = new List<int>();
            List<int> negative = new List<int>();
            List<string> dates = new List<string>();
            APIDataModel data = new APIDataModel();
            data.keyword = _Instance.GetKeywordFromID(keywordID).keyword;
            string query = "select posCount,negCount,dateCreated from DailyRecords where keywordID = @keywordID and dateCreated > @date order by dateCreated desc";
            
            string connection_string = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;

                    command.Parameters.AddWithValue("@keywordID", keywordID);
                    command.Parameters.AddWithValue("@date", date);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            positive.Add((int)reader["posCount"]);
                            negative.Add((int)reader["negCount"]);
                            dates.Add(((DateTime)reader["dateCreated"]).ToShortDateString());
                        }
                    }
                }
                connection.Close();
            }
            data.posList = positive.ToArray();
            data.negList = negative.ToArray();
            data.dates = dates.ToArray();

            List<int> volume = new List<int>();
            int pos;
            int neg;
            double tempAvg;
            List<double> avg = new List<double>();
            for (int i = 0; i < dates.Count; i++)
            {
                pos = positive[i];
                neg = negative[i];
                volume.Add(neg + pos);

                if (pos + neg > 0)
                    tempAvg = ((double)pos - neg) / (pos + neg);
                else
                    tempAvg = 0;

                avg.Add(tempAvg);
            }

            data.normalizedMood = avg.ToArray();
            data.totalHits = volume.ToArray();
            data.AnnotationIDs = new int[0];
            data.annotations = new string[0];
            data.annotationTitles = new string[0];
            return data;

        }
        public void DeleteAnnotation(int id)
        {
            string connectionString = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[DeleteAnnotation]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }

            }
        }
        public void CalculateAverage()
        {
            string connectionString = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[CollectTotals]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }

            }
        }
        public void DeleteOldData()
        {
            try
            {
                string connectionString = ServerConfiguration.GetConnectionString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 0;
                        command.CommandText = "[dbo].[DeleteOldData]";
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                    }

                }
            }
            catch
            {
                LogError("Error deleting old data");
            }
        }
        public void SetLatestTwitterID(string keyword,long id)
        {
            string connectionString = ServerConfiguration.GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[SetLatestTwitterID]";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@twitterID", id);
                    command.Parameters.AddWithValue("@keyword", keyword);
                    command.ExecuteNonQuery();
                }

            }
        }

        public long GetLatestTwitterID(string keyword)
        {
            long id;

            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "dbo.GetLatestTwitterID";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@keyword",keyword);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        id = (long)reader["LatestTwitterID"];
                    }
                }
                connection.Close();
            }

            return id;
        }
        public APIDataModel GetWidgetData(string keyword)
        {
            APIDataModel data = new APIDataModel { keyword = keyword, numDays = 0 };
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = 
@"
SELECT TOP 1 poscount, 
             negcount, 
             dailyrecords.datemodified 
FROM   dailyrecords 
       INNER JOIN keywords 
               ON keywords.id = dailyrecords.keywordid 
WHERE  keyword = @keyword
ORDER  BY dailyrecords.datemodified DESC 
";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@keyword", keyword);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            data.numDays = (int)reader["poscount"] + (int)reader["negcount"];
                            data.dates = new string[] { reader["datemodified"].ToString() };
                        }
                        
                    }
                }
                connection.Close();
            }

            return data;

        }
    }
}