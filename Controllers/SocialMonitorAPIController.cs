using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebMatrix.WebData;
using SocialMonitorCloud.Models;
using System.Globalization;
using SocialMonitorCloud.Sources;

namespace SocialMonitorCloud.Controllers
{
    // An API used by other services to retrieve analytical social media data whih is
    // then presented to the user using Google Charts.
    public class SocialMonitorController : ApiController
    {
        // GET api/values.
        [AllowAnonymous]
        [AllowWebApiCORS]
        public string Get()
        {
            return String.Empty;
        }
        // Basic commands for starting, stopping, or checking the status of the service
        [AllowAnonymous]
        public string Get(string query)
        {
            if (query == "toggle")
            {
                if (DataCollector.Instance.toggle())
                {
                    return "running";
                }
                else
                {
                    return "stopped";
                }
            }
            else if (query == "status")
            {
                if (DataCollector.Instance.GetStatus())
                {
                    return "running";
                }
                else
                {
                    return "stopped";
                }
            }
            else if (query == "start")
            {
                DataCollector.Instance.Start();
                return "started";
            }
            else if (query == "tracks")
            {
                return DataCollector.Instance.GetTracks();
            }
            else if (query == "killtwitter")
            {
                TwitterStream.Instance.Stop();
                return "OK";
            }
            else if (query == "restarttwitter")
            {
                TwitterStream.Instance.Restart();
                return "OK";
            }
            return "error";
        }
        // Returns social media data based on the given keyword and time range.
        [AllowAnonymous]
        public APIDataModel Get(
            string request, 
            string keyword="", 
            int runningAvg=0, 
            int interval=0,
            long sinceID=0,
            string id="",
            string unit="",
            string date="",
            string source=""
            )
        {
            int keyID = -1;
            APIDataModel data = new APIDataModel();
            if (request == "TOTAL")
            {
                if (runningAvg > 1) 
                {
                    d = ChartManager.collectAverages(keyword, runningAvg);
                }
                else
                {
                    data = ChartManager.CollectRawValues(keyword);
                    data.chartDiv = data.keyword.Replace(" ", String.Empty) + "ChartDiv";
                }
            }
            else if (request == "FIVE_MINUTES")
            {
                keyID = DatabaseAccessLayer.Instance.GetKeywordID(keyword);
                data = DatabaseAccessLayer.Instance.Get5Min(keyID);
            }

            else if (request == "USER_INFO")
            {
                data.keywords = AccountManager.GetUserKeywords(Guid.Parse(id));
                data.accounts = new Account[1];
                data.accounts[0] = AccountManager.GetAccountInfo(Guid.Parse(id));
            }
            else if (request == "USER_LIST")
            {
                data.accounts = AccountManager.GetAccounts().ToArray();
            }
            else if (request == "CLASSIFY_SENTIMENT")
            {
                data.ourSentiment = SentimentClassificationService.Instance.Classify(keyword);
                data.viralSentiment = "none";//json["mood"];
            }
            else if (request == "GET_ANNOTATIONS")
            {
                data = DatabaseAccessLayer.Instance.GetAnnotations(keyword);
            }
            else if (request == "TIME_RANGE")
            {
                keyID = DatabaseAccessLayer.Instance.GetKeywordID(keyword);
                if (unit == "5min")
                {
                    data = DatabaseAccessLayer.Instance.Get5Min(keyID);
                }
                else if (unit == "1min")
                {
                    d = DatabaseAccessLayer.Instance.GetLastMinute(keyID);
                }
                else if (unit == "10sec")
                {
                    data = DatabaseAccessLayer.Instance.GetTenSeconds(keyID,source);
                }
                else
                {
                    data = DatabaseAccessLayer.Instance.GetTimeRange(keyID, unit);
                    if (data.dates.Count() > 0)
                    {
                        data.sentimentList = DatabaseAccessLayer.Instance.GetHistoryPage(keyword, 0, DateTime.Parse(data.dates[0])).sentimentList;
                    }
                }
            }
            else if (request == "PAGE")
            {

                data = DatabaseAccessLayer.Instance.GetHistoryPage(keyword, interval-1, DateTime.Parse(date));
            }
            else if (request == "DAY_VALUES")
            {
                data = DatabaseAccessLayer.Instance.GetDayValues(keyword, DateTime.Parse(date));
            }
            else if(request=="WIDGET")
            {
                data = DatabaseAccessLayer.Instance.GetWidgetData(keyword);
            }

            return data;
        }
        // Posts to update user info, and add keywords to be tracked by the service.
        [AllowAnonymous]
        public void Post(
            string request,
            string userID="",
            string UserID="",
            string companyName="",
            string phoneNumber="",
            string email="",
            string address1="",
            string address2 = "",
            string address3 = "",
            string city = "",
            string state = "",
            string zip = "",
            string keyword = "",
            string title = "",
            string caption = "",
            string date = ""
            )
        {
            if (request == "UPDATE_ADDRESS")
            {
                Account account = new Account(Guid.Parse(userID));
                account.companyName = companyName;
                account.phoneNumber = phoneNumber;
                account.email = email;
                account.address1 = address1;
                account.address2 = address2;
                account.address3 = address3;
                account.city = city;
                account.state = state;
                account.zip = zip;
                account.FixNullParams();
                //account.country = country;
                //account.apartmentNum = apartmentNum;
                AccountManager.UpdateAccountInfo(account);
            }
            if (request == "ADD_KEYWORD")
            {

                TextInfo txtInfo = new CultureInfo("en-US", false).TextInfo;
                keyword = txtInfo.ToTitleCase(keyword);
                AccountManager.AssignKeyword(Guid.Parse(UserID), keyword);
                DataCollector.Instance.AddKeyword(keyword);
            }
            else if (request == "REMOVE_KEYWORD")
            {
                AccountManager.UnAssignKeyword(Guid.Parse(UserID), keyword);
                DataCollector.Instance.RemoveKeyword(keyword);
            }
            else if (request == "DELETE_ANNOTATION")
            {
                DatabaseAccessLayer.Instance.DeleteAnnotation(int.Parse(UserID));
            }
            else if (request == "INSERT_ANNOTATION")
            {
                ChartManager.InsertAnnotation(keyword, title, caption, date);
            }
            
            else if (User.Identity.IsAuthenticated && User.Identity.Name == "Administrator")
            {
                if (request == "DISABLE_KEY")
                {
                    ChartManager.DisableKeyword(keyword);
                }
                else if (request == "ENABLE_KEY")
                {
                    ChartManager.EnableKeyword(keyword);
                }
                else if (request == "DISABLE_ACCOUNT")
                {
                    AccountManager.DisableAccount(Guid.Parse(keyword));
                }
                else if (request == "ENABLE_ACCOUNT")
                {
                    AccountManager.EnableAccount(Guid.Parse(keyword));
                }
            }
        }
        [Authorize(Users = "Administrator")]
        public void Delete(string request,string userID)
        {
            if (request == "DELETE_USER")
            {
                AccountManager.DeleteAccount(int.Parse(userID));
            }
        }
    }
}