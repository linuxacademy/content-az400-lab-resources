using Newtonsoft.Json;
using System;
using System.Net;

namespace HealthClinic.Shared
{
    public static class APIConstants
    {
        public const string GetFoodLogsUrl = "https://mercuryhealth-dev.azurewebsites.net/api/FoodLogApi";
        public const string PostFoodUrl = "https://abelmercuryhealthservice-dev.azurewebsites.net/ImageIDAPI/UploadFoodImage1";
        public const string DeleteFoodLogUrl = "https://abelmercuryhealthservice-dev.azurewebsites.net/FoodApi/DeleteFoodItem";

        public static string buildNumber = "__Build_BuildNumber__";

        public static string GetTheFoodLogsUrl
        {

            get
            {
                try
                {
                    var url = "https://mhinfratoolsfunction.azurewebsites.net/api/MobileEnvRetriever?appname=mercuryhealth&buildnumberservicenameid=" + buildNumber + "getfoodlogsurl";
                    var result = new WebClient().DownloadString(url);
                    var mobileServiceEnv = JsonConvert.DeserializeObject<MobileServiceEnv>(result);

                    return mobileServiceEnv.Url;
                }
                catch (Exception) { }

                return GetFoodLogsUrl;
            }
        }

        public static string GetServiceIcon
        {
            get
            {
                var url = "https://mhinfratoolsfunction.azurewebsites.net/api/MobileEnvRetriever?appname=mercuryhealth&buildnumberservicenameid=" + buildNumber + "getfoodlogsurl";
                var result = new WebClient().DownloadString(url);
                var mobileServiceEnv = JsonConvert.DeserializeObject<MobileServiceEnv>(result);
                return mobileServiceEnv.Environment;
            }
        }
    }

    public class MobileServiceEnv
    {
        public string Environment { get; set; }
        public string Url { get; set; }
        public string partitionKey { get; set; }
        public string rowKey { get; set; }
        public DateTime Timesteamp { get; set; }
        public string Etag { get; set; }
    }
}
