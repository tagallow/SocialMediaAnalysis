using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialMonitorCloud.Models;
using WebMatrix.WebData;

namespace SocialMonitorCloud.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Analyze Phrases";
            ViewBag.Title = "Analyze Phrases";
            if (!AccountManager.AccountExists(User.Identity.Name))
            {
                AccountManager.CreateAccount(-1, User.Identity.Name, String.Empty);
            }
            ViewBag.UserID = AccountManager.getLocalAccountID(User.Identity.Name).ToString();
            if (!User.Identity.Name.Equals("Administrator"))
            {
                return View();
            }
            else
            {
                return View("Admin");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [Authorize]
        public new ActionResult Profile()
        {
            ViewBag.Message = "Your profile.";
            ViewBag.UserID = AccountManager.getLocalAccountID(User.Identity.Name).ToString();

            return View();
        }
        [Authorize]
        public ActionResult Detail(int keywordID)
        {
            ViewBag.Message = "Keyword";
            ViewBag.UserID = AccountManager.getLocalAccountID(User.Identity.Name).ToString();

            KeywordModel model = DatabaseAccessLayer.Instance.GetKeywordFromID(keywordID);

            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Annotation(string keyword, string userID)
        {
            ViewBag.keyword = keyword;
            ViewBag.userid = userID;

            return View();
        }
        //[Authorize(Roles="Admin")]
        [Authorize(Users = "Administrator")]
        public ActionResult Admin()
        {
            ViewBag.Message = "Your profile.";

            return View();
        }

        [Authorize(Users = "Administrator")]
        public ActionResult Charts()
        {
            ViewBag.Message = "Admin Chart Management";

            return View();
        }
        //[Authorize(Users = "tom")]
        public ActionResult Classify()
        {
            ViewBag.Message = "Classify phrases";

            return View();
        }
    }
}
