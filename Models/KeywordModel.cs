using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMonitorCloud.Models
{
    public class KeywordModel : IComparable<KeywordModel>
    {
        public string keyword;
        public bool isActive;
        public string dateModified;
        public int ID;
        public string chartDiv;

        public int CompareTo(KeywordModel comparePart)
        {
            return this.keyword.CompareTo(comparePart.keyword);
        }
    }
}