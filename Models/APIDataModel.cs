using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMonitorCloud.Models
{
    public class APIDataModel
    {
        public int[] posList { get; set; }
        public int[] negList { get; set; }
        public int[] analyzedCount { get; set; }
        public int numDays { get; set; }
        public Account[] accounts { get; set; }
        public List<Sentiment> sentimentList { get; set; }
        public string[] userKeywords { get; set; }
        public string[] dates { get; set; }
        public int[] AnnotationIDs { get; set; }
        public string[] annotationTitles { get; set; }
        public string[] annotations { get; set; }
        public string keyword { get; set; }
        //public int posCount { get; set; }
        //public int negCount { get; set; }
        public double[] normalizedMood { get; set; }
        public int[] totalHits { get; set; }
        public bool languageStatus { get; set; }
        public int viralHeatQueries { get; set; }
        //public Dictionary<string, bool> keywords { get; set; }
        public List<KeywordModel> keywords { get; set; }
        public long lastTwitterID { get; set; }
        public String lastTwitterIDString { get; set; }
        public string chartDiv { get; set; }
        public string ourSentiment { get; set;}
        public string viralSentiment { get; set; }
    }
}