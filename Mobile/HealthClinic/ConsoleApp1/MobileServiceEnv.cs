using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class MobileServiceEnv
    {
        public string EnvironmentName { get; set; }
        public string Url { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Etag { get; set; }

    }
}
