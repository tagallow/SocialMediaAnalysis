using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMonitorCloud.Models
{
    /// <summary>
    /// Used or parsing viralheat json responses
    /// </summary>
    public class ViralHeatData
    {
        public int quota_remaining { get; set; }
    }
    public static class ViralHeat
    {
        /// <summary>
        /// Gets the mood of a post using the viralheat API
        /// </summary>
        /// <param name="text">The post to be analyzed</param>
        /// <returns>"negative" or "positive"</returns>
        public static Sentiment GetSentimentMood(string text)
        {
            Sentiment result = new Sentiment();

            string viralHeatApiKey = "&api_key=" + ChartManager.GetViralHeatAPIKey();
            //if(firstAPIUsed)
            //viralHeatApiKey = "&api_key=olGbEN1IUhKjrvgQvZOt";
            string url = "http://www.viralheat.com/api/sentiment/review.json?text=" + text + viralHeatApiKey;

            string jsonString;
            try
            {
                jsonString = new System.Net.WebClient().DownloadString(url);
            }
            catch (Exception e)
            {
                //Error with the webservice
                System.Diagnostics.Debug.WriteLine("========================");
                System.Diagnostics.Debug.WriteLine("Error with webservice");
                System.Diagnostics.Debug.WriteLine(e.ToString());
                System.Diagnostics.Debug.WriteLine("========================");
                //done = true;
                return null;
            }
            if (jsonString.Contains("Error"))
            {
                //Post was probably not in english, skip it

                if (jsonString.Contains("Over quota limit"))
                {
                    //firstAPIUsed = true;
                    //ChartManager.toggleLanguageAnalysis();
                    System.Diagnostics.Debug.WriteLine("=================================");
                    System.Diagnostics.Debug.WriteLine("Out of ViralHeat Queries");
                    System.Diagnostics.Debug.WriteLine("=================================");
                    result = new Sentiment();
                    result.mood = "unknown";
                    result.prob = -1;
                    result.text = text;
                    result = null;
                }
                else
                {
                    //ChartManager.setLanguageAnalysis(false);
                    System.Diagnostics.Debug.WriteLine("========================");
                    System.Diagnostics.Debug.WriteLine("Error with webservice");
                    System.Diagnostics.Debug.WriteLine(url);
                    System.Diagnostics.Debug.WriteLine(jsonString);
                    System.Diagnostics.Debug.WriteLine("========================");
                    result = null;
                }
            }
            else
            {
                result = JsonConvert.DeserializeObject<Sentiment>(jsonString);
            }

            return result;
        }
    }
    public class Statistics
    {
    }
}