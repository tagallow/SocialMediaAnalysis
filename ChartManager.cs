using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json;
using SocialMonitorCloud.Models;

namespace SocialMonitorCloud
{
    /// <summary>
    /// Manages various functions and data regarding charts
    /// </summary>
    public static class ChartManager
    {
        public static APIDataModel CollectRawValues(string keyword)
        {
            APIDataModel d = DatabaseAccessLayer.Instance.CollectRawData(keyword);
            d.keyword = keyword;

            List<int> volume = new List<int>();
            int pos;
            int neg;
            double tempAvg;
            List<double> avg = new List<double>();
            for(int i=0;i<d.dates.Length;i++)
            {
                pos = d.posList[i];
                neg = d.negList[i];
                volume.Add(neg+pos);

                if (pos + neg > 0)
                    tempAvg = ((double)pos - neg) / (pos + neg);
                else
                    tempAvg = 0;

                avg.Add(tempAvg);
            }

            d.normalizedMood = avg.ToArray();
            d.totalHits = volume.ToArray();

            return d;
        }
        public static int MinuteVolume(string keyword, TimeSpan interval)
        {
            TimeSpan upperBound = TimeSpan.FromMinutes(5);
            TimeSpan lowerBound = TimeSpan.FromSeconds(upperBound.Seconds + interval.Seconds);
            int result = DatabaseAccessLayer.Instance.GetMinuteVolume(keyword, upperBound, lowerBound);
            return result;
        }

        public static double MinuteMood(string keyword, TimeSpan interval)
        {
            TimeSpan upperBound = TimeSpan.FromMinutes(5);
            TimeSpan lowerBound = TimeSpan.FromSeconds(upperBound.Seconds + interval.Seconds);

            return DatabaseAccessLayer.Instance.GetMinuteMood(keyword, upperBound, lowerBound);
        }
        public static APIDataModel CollectAverages(string keyword, int interval)
        {
            APIDataModel d = DatabaseAccessLayer.Instance.CollectRawData(keyword);
            DateTime end = DateTime.Today.AddDays(-1);
            List<double> avg = new List<double>();
            List<int> volume = new List<int>();
            List<string> dates = new List<string>(d.dates);
            List<int> posCount = new List<int>(d.posList);
            List<int> negCount = new List<int>(d.negList);

            if (String.IsNullOrEmpty(keyword))
            {
                keyword = Guid.NewGuid().ToString();
            }

            d.keyword = keyword;

            for (int i = 0; i < interval; i++)
            {
                avg.Add(0);
                volume.Add(0);
            }
            for (int i = interval; i < dates.Count; i++)
            {
                if (ValidRange(dates.GetRange(i - interval, interval)))
                {
                    avg.Add(Average(posCount.GetRange(i - interval, interval), negCount.GetRange(i - interval, interval)));
                    volume.Add(posCount[i] + negCount[i]);
                }
                else
                {
                    avg.Add(0);
                    volume.Add(0);
                }
            }
            d.normalizedMood = avg.ToArray();
            d.totalHits = volume.ToArray();

            return d;
        }
        private static Boolean ValidRange(List<string> dates)
        {
            Boolean result = true;
            List<DateTime> dateList = new List<DateTime>();
            DateTime previousDay;
            foreach (string s in dates)
            {
                dateList.Add(DateTime.Parse(s));
            }
            previousDay = dateList[0];
            dateList.RemoveAt(0);
            foreach (DateTime d in dateList)
            {
                if (d.DayOfYear - previousDay.DayOfYear != 1)
                {
                    result = false;
                }
                previousDay = d;
            }

            return result;

        }
        private static double Average(List<int> posCount, List<int> negCount)
        {
            double result = 0;
            int posSum = 0;
            int negSum = 0;

            foreach (int i in posCount)
            {
                posSum += i;
            }
            foreach (int i in negCount)
            {
                negSum += i;
            }
            result = ((double)posSum - negSum) / (posSum + negSum);

            return result;
        }
        public static string GetViralHeatAPIKey()
        {
            string key = "Pmpi1P5B35iQ4G5N3a";
            //key = "fSQABsRIXR4GFrDiHozX";
            return key;
        }
        /// <summary>
        /// Queries the viralheat api for how many requests our API key has left
        /// </summary>
        /// <returns>The number of requests remaining</returns>
        public static int ViralHeatQueries()
        {
            int result = -1;
            string url = "https://www.viralheat.com/api/sentiment/quota.json?api_key=" + GetViralHeatAPIKey();
            string jsonString;

            ViralHeatData d = new ViralHeatData();
            jsonString = new System.Net.WebClient().DownloadString(url);
            d = JsonConvert.DeserializeObject<ViralHeatData>(jsonString);
            result = d.quota_remaining;

            return result;
        }
        /// <summary>
        /// Returns whether or not a key already exists in the database
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool DoesKeyExist(string key)
        {
            bool result = false;
            List<KeywordModel> keyList = GetKeywords();
            foreach (KeywordModel k in keyList)
            {
                if (k.keyword.Equals(key))
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Generates a list of all keywords in the database
        /// </summary>
        /// <returns></returns>
        public static List<KeywordModel> GetKeywords()
        {
            return DatabaseAccessLayer.Instance.GetKeywords();
        }
        /// <summary>
        /// Enables a keyword for data collection
        /// </summary>
        /// <param name="keyword"></param>
        public static void EnableKeyword(string keyword)
        {
            DatabaseAccessLayer.Instance.EnableKeyword(keyword);
        }
        /// <summary>
        /// Disables a key from data collection
        /// </summary>
        /// <param name="keyword"></param>
        public static void DisableKeyword(string keyword)
        {
            DatabaseAccessLayer.Instance.DisableKeyword(keyword);
        }
        
        public static APIDataModel GetCSV(string keyword)
        {
            APIDataModel d = new APIDataModel();

            return d;
        }
        public static void InsertAnnotation(string Keyword, string Title, string Caption, string Date)
        {
            DatabaseAccessLayer.Instance.InsertAnnotation(Keyword, Title, Caption, Date);
        }
        public static bool IsActive(string keyword)
        {
            return DatabaseAccessLayer.Instance.IsActive(keyword);
        }
        public static APIDataModel GetHistoryPage(string keyword, int page, DateTime latestDate)
        {
            return DatabaseAccessLayer.Instance.GetHistoryPage(keyword, page, latestDate);
        }

    }
}