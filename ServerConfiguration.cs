using System;
using System.Configuration;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace SocialMonitorCloud
{
    public static class ServerConfiguration
    {
        public static string GetConnectionString()
        {
            string connectionString = "";

            if (RoleEnvironment.IsAvailable)
                connectionString = RoleEnvironment.GetConfigurationSettingValue("ConnectionStringCloud_Dev");
            else
                connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringDev"].ConnectionString;


            return connectionString;
        }
        public static string GetDictionaryConnectionString()
        {
            string connectionString = "";

            if (RoleEnvironment.IsAvailable)
                connectionString = RoleEnvironment.GetConfigurationSettingValue("DictionaryConnectionString");
            else
                connectionString = ConfigurationManager.ConnectionStrings["DictionaryConnectionString"].ConnectionString;

            return connectionString;
        }
    }
}