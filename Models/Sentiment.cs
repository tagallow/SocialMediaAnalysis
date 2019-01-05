using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMonitorCloud
{
    public class Sentiment
    {
        public float prob { get; set; }
        public string text { get; set; }
        public string mood { get; set; }
        public DateTime time { get; set; }
        public string timeString { get; set; }
        public string source { get; set; }
        public string keyword { get; set; }
        public string author { get; set; }
        public long twitterID { get; set; }
        public int sourceID { get; set; }

        public void setTime(string t)
        {
            time = DateTime.Parse(t);
        }

        public override string ToString()
        {
            string result = "";
            result += "Keyword: " + keyword + "\n";
            result += "Source: " + source + "\n";
            result += "Author: " + author + "\n";
            result += "Time: " + time.ToString() + "\n";
            result += "Mood: " + mood + "\n";
            result += "Probability: " + prob.ToString("P") + "\n";
            result += "Text: " + text + "\n";
            return result;
        }

        public override bool Equals(object obj)
        {
            Sentiment s = (Sentiment)obj;
            return text.Equals(s.text);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
