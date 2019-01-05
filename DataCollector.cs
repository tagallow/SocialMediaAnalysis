using SocialMonitorCloud.Sources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace SocialMonitorCloud
{
    /// <summary>
    /// This class connects to the facebook, twitter, and google plus APIs
    /// and gathers data about each keyword activated in the database
    /// </summary>
    public class DataCollector
    {
        private bool _IsRunning;
        //private Timer _UpdateAverageTimer = new Timer();
        private Timer _CollectKeywordsTimer = new Timer();
        private ConcurrentDictionary<string, Task> m_KeywordTasks = new ConcurrentDictionary<string, Task>();
        private ConcurrentDictionary<string, DateTime> m_LastScanTime = new ConcurrentDictionary<string, DateTime>();
        private List<ISentimentSource> m_SentimentSources = new List<ISentimentSource>();
        private static readonly object syncroot = new object();

        private static DataCollector _Instance = null;

        protected DataCollector()
        {
            _IsRunning = false;
            //m_SentimentSources.Add(new FacebookSentimentSource());
            //m_SentimentSources.Add(new GoogleSentimentSource());
            //m_SentimentSources.Add(new TwitterSentimentSource());
            //TwitterStream.Instance.LoadKeywords();
        }

        public static DataCollector Instance
        {
            get
            {
                lock (syncroot)
                {
                    if (_Instance == null)
                    {
                        _Instance = new DataCollector();
                    }
                }
                return _Instance;
            }
        }

        private void Initialize()
        {
            TwitterStream.Instance.Start();
            m_KeywordTasks.Clear();
            DatabaseAccessLayer.Instance.DeleteOldData();
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetConnectionString()))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "[dbo].[GetActiveKeywords]";
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string keyword = (string)reader["Keyword"];
                            AddKeyword(keyword);
                            //m_KeywordTasks[keyword].Start();
                            //System.Diagnostics.Debug.WriteLine("Collecting data for " + keyword);
                        }
                    }
                }
            }
        }

        public void AddKeyword(string keyword)
        {
            TwitterStream.Instance.AddTrack(keyword);
            if (!m_KeywordTasks.ContainsKey(keyword))
            {
                TimeSpan delay;
                m_KeywordTasks[keyword] = new Task(delegate()
                    {
                        while (_IsRunning && m_KeywordTasks.ContainsKey(keyword))
                        {
                            delay = TimeSpan.FromSeconds(5);
                            foreach (ISentimentSource sentimentSource in m_SentimentSources)
                            {
                                try
                                {
                                    DateTime lastCall = DateTime.Now;
                                    if (!m_LastScanTime.ContainsKey(keyword))
                                        m_LastScanTime[keyword] = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
                                    List<Sentiment> sentiments = null;
                                    try
                                    {
                                        sentiments = sentimentSource.GetSentiment(keyword, m_LastScanTime[keyword]);
                                        InsertToDB(sentiments);
                                    }
                                    catch (Exception e)
                                    {
                                        if (e.Message.Equals("Too many requests"))
                                        {
                                            delay = TimeSpan.FromSeconds(15);
                                            System.Diagnostics.Debug.WriteLine("Too many requests " + keyword);
                                        }
                                        DatabaseAccessLayer.Instance.LogError(e);
                                    }
                                    
                                    m_LastScanTime[keyword] = lastCall;
                                }
                                catch(Exception e)
                                {
                                    System.Diagnostics.Debug.WriteLine("error in " + keyword);
                                    DatabaseAccessLayer.Instance.LogError(e);
                                }
                            }
                            Thread.Sleep(delay);
                        }
                        TwitterStream.Instance.RemoveTrack(keyword);
                        System.Diagnostics.Debug.WriteLine("Thread " + keyword + " is ending");
                    });
                m_KeywordTasks[keyword].Start();
            }
            
        }

        public void RemoveKeyword(string keyword)
        {
            TwitterStream.Instance.RemoveTrack(keyword);
            if (m_KeywordTasks.ContainsKey(keyword))
            {
                Task task;
                //m_KeywordTasks[keyword] = null;
                if (m_KeywordTasks.TryRemove(keyword, out task))
                {
                    //keyword removed
                }
                else
                {
                    m_KeywordTasks[keyword] = null;
                }
            }
        }
        public String GetTracks()
        {
            return TwitterStream.Instance.GetCurrentTracks();
        }
        public void Start()
        {
            _IsRunning = true;
            Initialize();

            _CollectKeywordsTimer.Interval = TimeSpan.FromMinutes(5).TotalMilliseconds;
            _CollectKeywordsTimer.Elapsed += (sender, e) =>
                {
                    _CollectKeywordsTimer.Stop();
                    try
                    {
                        DatabaseAccessLayer.Instance.CalculateAverage();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        DatabaseAccessLayer.Instance.LogError(ex);
                    }
                    //Initialize();
                    if(_IsRunning)
                        _CollectKeywordsTimer.Start();
                };
            _CollectKeywordsTimer.Start();
            //TwitterStream.Instance.LoadKeywords();
        }
        public void Restart()
        {
            Stop();
            Start();
            //TwitterStream.Instance.LoadKeywords();
        }

        public void Stop()
        {
            _IsRunning = false;
            m_KeywordTasks.Clear();
            //TwitterStream.Instance.Pause();
        }
        public bool toggle()
        {
            if (_IsRunning)
            {
                Stop();
            }
            else
            {
                Start();
            }
            return _IsRunning;
        }
        public bool GetStatus()
        {

            return _IsRunning;
        }
  
        /// <summary>
        /// Inserts a list of sentiments to the DB
        /// </summary>
        /// <param name="sentList">The list of sentiments</param>
        /// <param name="keyword">The keyword the list is about</param>
        private void InsertToDB(List<Sentiment> sentList)
        {
            foreach (Sentiment sent in sentList)
            {
                DatabaseAccessLayer.Instance.InsertSentiment(sent);
            }
            DatabaseAccessLayer.Instance.DeleteOldData();
        }
    }
}