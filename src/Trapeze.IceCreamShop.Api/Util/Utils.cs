using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trapeze.IceCreamShop.Api.Util
{
    public static class Utils
    {
        public static string GetUserName(HttpContext httpContext)
        {
            string authHeader = httpContext?.Request.Headers["Authorization"];
            string encodedUserNamePassword = authHeader.Substring("Basic".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string userNameAndPassword = encoding.GetString(Convert.FromBase64String(encodedUserNamePassword));
            int index = userNameAndPassword.IndexOf(":");
            var userName = userNameAndPassword.Substring(0, index);
            return userName;
        }
    }
}
