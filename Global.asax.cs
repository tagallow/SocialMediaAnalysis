using Microsoft.WindowsAzure.ServiceRuntime;
using SocialMonitorCloud.Controllers;
using SocialMonitorCloud.Sources;
using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;

namespace SocialMonitorCloud
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            FilterConfig.RegisterWebApiFilters(GlobalConfiguration.Configuration.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            //WebSecurity.InitializeDatabaseConnection(ServerConfiguration.GetConnectionString(), "System.Data.SqlClient", "UserProfile", "UserId", "UserName", true);

            try
            {

                SentimentClassificationService.Instance.Classify("initialize");
                DataCollector.Instance.Start();
                DatabaseSchemaUtility.InitializeSchema();
            }
            catch (Exception e)
            {
                DatabaseAccessLayer.Instance.LogError(e);
            }
            if (RoleEnvironment.IsAvailable)
            {
                //DataCollector.Instance.Start();
                
                //DatabaseSchemaUtility.InitializeSchema();
            }
            
        }
    }

    /// <summary>
    /// Attribute for web api actionresults
    /// </summary>
    public class AllowWebApiCORSAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            string urlConfigValue = ConfigurationManager.AppSettings["AllowUrls"];
            if (!string.IsNullOrEmpty(urlConfigValue))
            {
                if (!actionExecutedContext.Response.Headers.Contains("Access-Control-Allow-Origin"))
                {
                    actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", urlConfigValue);
                }
            }
        }
    }

    /// <summary>
    /// Attribute for mvc calls
    /// </summary>
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string urlConfigValue = ConfigurationManager.AppSettings["AllowUrls"];
            if (!string.IsNullOrEmpty(urlConfigValue))
            {
                
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", urlConfigValue);
            }
            
            base.OnActionExecuted(filterContext);
        }

      
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
    }

}