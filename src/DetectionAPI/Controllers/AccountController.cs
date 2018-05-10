using DetectionAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;

namespace DetectionAPI.Controllers
{
    public class AccountController : ApiController
    {
        [HttpGet]
        [Route("api/account/token/new")]
        public IHttpActionResult TokenNew()
        {
            return NotFound();
        }

        [HttpGet]
        [Authorize(Roles = "Administrators, CustomRoles")]
        [Route("api/account/token/validate")]
        public IHttpActionResult TokenValidate()
        {
            return NotFound();
        }

        [HttpGet]
        [Route("api/account/token/current")]
        public IHttpActionResult TokenCurrent()
        {
            return NotFound();
        }


        private void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }



        public class MyPrincipal : IPrincipal
        {
            public IIdentity Identity => throw new NotImplementedException();

            public bool IsInRole(string role)
            {
                throw new NotImplementedException();
            }
        }


        public class MyCustomAuthorization : IAuthorizationFilter
        {
            public bool AllowMultiple => throw new NotImplementedException();

            public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
            {
                throw new NotImplementedException();
            }
        }

        public class MyCustomAuthFilterAttribute : AuthorizationFilterAttribute
        {

        }


        /// <summary>
        /// Trying to make an authorization request
        /// </summary>
        /// <returns></returns>
        /// 

        [Authorize]
        public IHttpActionResult Authorize()
        {

            return Ok();
        }

        


    }
}
