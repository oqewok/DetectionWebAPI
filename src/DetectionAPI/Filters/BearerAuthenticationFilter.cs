using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DetectionAPI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class BearerAuthenticationFilter : AuthorizationFilterAttribute
    {
        bool Active = true;

        public BearerAuthenticationFilter()
        {

        }

        public BearerAuthenticationFilter(bool active)
        {
            Active = active;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (Active)
            {
                var identity = ParseAuthorizationHeader(actionContext);
                if (identity == null)
                {
                    Challenge(actionContext);
                    return;
                }

                if(!OnAuthorizeUser(identity.Name, actionContext))
                {
                    Challenge(actionContext);
                    return;
                }

                var principal = new GenericPrincipal(identity, null);

                Thread.CurrentPrincipal = principal;

                // inside of ASP.NET this is required
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }

                base.OnAuthorization(actionContext);
            }
        }

        protected virtual bool OnAuthorizeUser(string token, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            return true;
        }

        protected virtual BearerAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            string authHeader = null;
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null && auth.Scheme == "Bearer")
                authHeader = auth.Parameter;

            if (string.IsNullOrEmpty(authHeader))
                return null;
            
            return new BearerAuthenticationIdentity(authHeader);
        }

        void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", $@"Bearer \{host}\");
        }
    }
}
