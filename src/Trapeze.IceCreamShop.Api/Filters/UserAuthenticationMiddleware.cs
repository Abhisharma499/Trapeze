using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trapeze.IceCreamShop.Data.Entities;

namespace Trapeze.IceCreamShop.Api.Filters
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class UserAuthenticationMiddleware
    {
        List<UserInformation> users = new List<UserInformation>()
        {
            new UserInformation() {UserName = "amosvani",Password = "TW9zdmFuaXh4", FullName = "Alanna Mosvani"},
            new UserInformation() {UserName = "mcauthon",Password = "Q2F1dGhvbnh4", FullName = "Mat Cauthon" },
            new UserInformation() {UserName = "mdamodred",Password = "RGFtb2RyZWR4", FullName = "Moiraine Damodred" },
        };

        private readonly RequestDelegate _next;

        public UserAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string authHeader = httpContext?.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic"))
            {
                string encodedUserNamePassword = authHeader.Substring("Basic".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                string userNameAndPassword = encoding.GetString(Convert.FromBase64String(encodedUserNamePassword));
                int index = userNameAndPassword.IndexOf(":");
                var userName = userNameAndPassword.Substring(0, index);
                var password = userNameAndPassword.Substring(index + 1);

                // Replace this with your own system of security / means of validating credentials
                var isValid = users.Any(x => x.UserName == userName && x.Password == password);

                if (isValid)
                {
                    await _next.Invoke(httpContext).ConfigureAwait(false);
                }
                else
                {
                    httpContext.Response.StatusCode = 401;
                    return;
                }
            }
            else
            {
                httpContext.Response.StatusCode = 401;
                return;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserAuthenticationMiddleware>();
        }
    }
}
