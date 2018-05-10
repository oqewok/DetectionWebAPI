using DetectionAPI.Filters;
using DetectionAPI.Models;
using Newtonsoft.Json;
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

        [HttpGet]
        [RealBasicAuthenticationFilter]
        [Route("api/account/auth")]
        public IHttpActionResult Authorize()
        {
            return Ok("You have been authorized");
        }


        [HttpGet]
        [Route("api/account/base64")]
        public IHttpActionResult GetBase64String()
        {
            string original = "oqewok:1234";

            var base64str = new MyBase64Str();
            base64str.Base64EncodedString = Base64Encode(original);

            return Ok(base64str);
        }

        
        public class MyBase64Str
        {
            [JsonProperty (PropertyName = "string_based64")]
            public string Base64EncodedString { get; set; }

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }


    }
}
