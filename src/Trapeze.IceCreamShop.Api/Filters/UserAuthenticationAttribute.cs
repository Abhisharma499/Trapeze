namespace Trapeze.IceCreamShop.Api.Filters
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Collections.Generic;
    using Trapeze.IceCreamShop.Data.Entities;
    using System.Linq;

    public class UserAuthenticationAttribute: AuthorizationFilterAttribute
    {
        List<UserInformation> users = new List<UserInformation>()
        {
            new UserInformation() {UserName = "amosvani",Password = "TW9zdmFuaXh4", FullName = "Alanna Mosvani"},
            new UserInformation() {UserName = "mcauthon",Password = "Q2F1dGhvbnh4", FullName = "Mat Cauthon" },
            new UserInformation() {UserName = "mdamodred",Password = "RGFtb2RyZWR4", FullName = "Moiraine Damodred" },
        };

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                var decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                var usernamePasswordArray = decodedAuthenticationToken.Split(':');
                var userName = usernamePasswordArray[0];
                var password = usernamePasswordArray[1];

                // Replace this with your own system of security / means of validating credentials
                var isValid = users.Any(x => x.UserName == userName && x.Password == password);

                if (isValid)
                {
                    var principal = new GenericPrincipal(new GenericIdentity(userName), null);
                    Thread.CurrentPrincipal = principal;

                    actionContext.Response =
                       actionContext.Request.CreateResponse(HttpStatusCode.OK,
                          "User " + userName + " successfully authenticated");

                    return;
                }
            }

            HandleUnathorized(actionContext);
        }

        private static void HandleUnathorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='Data' location = 'http://localhost:");
        }
    }
}
