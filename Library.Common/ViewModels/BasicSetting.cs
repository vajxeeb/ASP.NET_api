using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library.Core.ViewModels
{
    public static class BasicSetting
    {
        public static string PaymentApiUrl { get; set; }
        public static string ContentBaseUrl { get; set; }

        public static string FcmServerkey { get; set; }
        public static string FcmSenderID { get; set; }
        public static string WeekStart { get; set; }
        public static string WeekEnd { get; set; }

        public static string FromEmail { get; set; }
        public static string FromName { get; set; }
        public static string SMTP_USERNAME { get; set; }
        public static string SMTP_PASSWORD { get; set; }
        public static string SMTP_PORT { get; set; }
        public static string SMTP_HOST { get; set; }
        public static string FB_Key { get; set; }




    }
}
