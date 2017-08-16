using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3Console
{
    internal static class ProjConfig
    {
        public static string S3AccessKey = ConfigurationManager.AppSettings["AWS.AccessKey"] ?? string.Empty;
        public static string S3SecretKey = ConfigurationManager.AppSettings["AWS.SecretKey"] ?? string.Empty;
    }
}
