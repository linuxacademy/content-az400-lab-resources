using Newtonsoft.Json;
using System;
using System.Net;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string url = "https://mhinfratoolsfunction.azurewebsites.net/api/MobileEnvRetriever?appname=mercuryhealth&buildnumberservicenameid=12456.3";
            string result1 = new WebClient().DownloadString(url);
            var deserializedObj = JsonConvert.DeserializeObject<MobileServiceEnv>(result1);
        }
    }
}
