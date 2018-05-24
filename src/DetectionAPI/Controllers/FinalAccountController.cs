using DetectionAPI.Models;
using DetectionAPI.Filters;
using DetectionAPI.Detection;
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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using DetectionAPI.Detection.DetectionResult;
using PlateDetector.Detection;
using System.Diagnostics;
using Ninject;
using OpenCvSharp;
using DetectionAPI.Database;
using DetectionAPI.Database.Entities;
using System.Threading;

namespace DetectionAPI.Controllers
{
    public class FinalAccountController : ApiController
    {
        /// <summary>
        /// Create new user, FromBody parameter should be passed
        /// as raw application/json:
        /// {
        ///     username : "username@example.com",
        ///     password : "example_password" 
        /// }
        /// </summary>
        /// <param name="postedValues"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/f/account/new")]
        public IHttpActionResult AccountNew([FromBody] PostedUsernamePassword postedValues)
        {
            if (postedValues.Password.Length < 6 || postedValues.Password.Length > 30)
            {
                return BadRequest("Your password is too short, or too long");
            }

            if (!(new EmailAddressAttribute().IsValid(postedValues.Username)))
            {
                return BadRequest("Your e-mail is not valid");
            }

            using (var dbContext = new ApiDbContext())
            {
                var user = dbContext.Set<User>().Where(p => p.Username == postedValues.Username).ToList().FirstOrDefault();
                if (user != null)
                {
                    return BadRequest("User with the same username already exists!");
                }

                //Create new user after all checks
                else
                {
                    var newUser = new User
                    {
                        //TODO : access tokens are not implemented
                        AccessToken = "token1234",
                        CreationTime = DateTime.Now,
                        Password = postedValues.Password,
                        SessionId = -1,
                        Username = postedValues.Username,
                        UserType = 0,
                    };

                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();
                }
            }


            return Ok();
        }

        /// <summary>
        /// Returns token when FromBody parameter
        /// have been passed as raw application/json:
        /// {
        ///     username : "username@example.com",
        ///     password : "example_password" 
        /// }
        /// </summary>
        /// <param name="postedValues"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/f/account/token", Name = "GetAccessTokenRawJsonParameter")]
        //public IHttpActionResult Token([FromBody] PostedUsernamePassword postedValues)
        //{
          
        //    var tokenDict = new Dictionary<string, string>();
        //    tokenDict.Add("token", Guid.NewGuid().ToString());


        //    //routeValues can be formed like: new { username = postedValues.Username, password = postedValues.Password }
        //    //and appears as Location Header in response - %route%?username=Value&password=Value
        //    //content: dict values appears in body of response
        //    return CreatedAtRoute(routeName: "GetAccessTokenRawJsonParameter", routeValues: new {}, content: tokenDict);


        //    //!!!!!! BASE64 STRING IN HEADER !!!!!
        //    // so not like above with FromBody

        //    //check username/password

        //    //get or create new token

        //    //return token
        //}

        /// <summary>
        /// Returns token when basic auth credentials given
        /// Checks if user exists in filter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RealBasicAuthenticationFilter]
        [Route("api/f/account/token", Name = "GetAccessTokenHeaderParameter")]
        public IHttpActionResult Token()
        {
            var name = Thread.CurrentPrincipal.Identity.Name;
            var authType = Thread.CurrentPrincipal.Identity.AuthenticationType;
            var isAuthentificated = Thread.CurrentPrincipal.Identity.IsAuthenticated;


            var token = Guid.NewGuid().ToString();

            using(var dbContext = new ApiDbContext())
            {
                var certainUser = dbContext.Set<User>().Where(p => p.Username == name).ToList().FirstOrDefault();
                if(certainUser != null)
                {
                    certainUser.AccessToken = token;
                    dbContext.SaveChanges();
                }

            }

            var tokenDict = new Dictionary<string, string>();
            tokenDict.Add("token", token);
            return CreatedAtRoute(routeName: "GetAccessTokenHeaderParameter", routeValues: new {}, content: tokenDict);
        }


        public IHttpActionResult TokenRefresh()
        {
            return NotFound();
        }

        public IHttpActionResult AccountStats()
        {
            return NotFound();
        }


        public IHttpActionResult AccountLimits()
        {
            return NotFound();
        }



        public class PostedUsernamePassword
        {
            [Required, MinLength(6)]
            public string Username { get; set; }

            [Required, MinLength(6)]
            public string Password { get; set; }

            public override string ToString()
            {
                var s = $@"Username: {Username}, Password:{Password}";

                return s;
            }
        }




    }
}
