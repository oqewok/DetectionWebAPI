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
using System.Runtime.Serialization;

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
                        AccessToken = Guid.NewGuid().ToString("N"),
                        CreationTime = DateTime.Now,
                        Password = postedValues.Password,
                        SessionId = -1,
                        Username = postedValues.Username,
                        UserType = 0,
                    };

                    var newSession = new Session
                    {
                        CreationTime = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddMonths(1),
                        ImageCount = 0,
                        IsLimitReached = false,
                        SessionType = newUser.UserType,
                        PlatesCount = 0,
                        UserId = newUser.Id,
                        User = newUser
                    };

                    dbContext.Sessions.Add(newSession);
                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();

                    var updateUser = dbContext.Set<User>().Where(p => p.Username == postedValues.Username).ToList().LastOrDefault();
                    updateUser.SessionId = newSession.Id;

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

            var tokenDict = new Dictionary<string, string>();

            var message = "message";
            var token = "User does not exist";

            using (var dbContext = new ApiDbContext())
            {
                var certainUser = dbContext.Set<User>().Where(p => p.Username == name).ToList().FirstOrDefault();

                if(certainUser != null)
                {
                    message = "token";
                    token = certainUser.AccessToken;
                }
            }

            tokenDict.Add(message, token);
            return CreatedAtRoute(routeName: "GetAccessTokenHeaderParameter", routeValues: new {}, content: tokenDict);
        }

        [HttpPost]
        [RealBasicAuthenticationFilter]
        [Route("api/f/account/token/refresh", Name = "RefreshAccessTokenHeaderParameter")]
        public IHttpActionResult TokenRefresh()
        {
            var username = Thread.CurrentPrincipal.Identity.Name;
            var authType = Thread.CurrentPrincipal.Identity.AuthenticationType;
            var isAuthentificated = Thread.CurrentPrincipal.Identity.IsAuthenticated;

            var tokenDict = new Dictionary<string, string>();

            var message = "message";
            var token = "User does not exist";

            using (var dbContext = new ApiDbContext())
            {
                var certainUser = dbContext.Set<User>().Where(p => p.Username == username).ToList().FirstOrDefault();

                if (certainUser != null)
                {
                    message = "token";
                    token = Guid.NewGuid().ToString("N");
                    certainUser.AccessToken = token;
                    dbContext.SaveChanges();
                }
            }

            tokenDict.Add(message, token);
            return CreatedAtRoute(routeName: "GetAccessTokenHeaderParameter", routeValues: new { }, content: tokenDict);
        }

        public IHttpActionResult AccountStats()
        {
            return NotFound();
        }

        [HttpGet]
        [Route("api/f/account/limits")]
        [RealBearerAuthenticationFilter]
        public IHttpActionResult AccountLimits()
        {
            var authorizedUserToken = Thread.CurrentPrincipal.Identity.Name;
            long id = 0;

            using (var dbContext = new ApiDbContext())
            {
                var user = dbContext.Set<User>().Where(p => p.AccessToken == authorizedUserToken).ToList().LastOrDefault();

                if (user != null)
                {
                    id = user.Id;
                }
            }

            var currentLimitInfo = CheckLimitByUserId(id);

            return Ok(currentLimitInfo);
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

        [DataContract]
        public class AvailableLimits
        {
            [DataMember]
            [JsonProperty(PropertyName = "limitReached")]
            public bool IsLimitReached { get; set; }

            [DataMember]
            [JsonProperty(PropertyName = "imagesCount")]
            public long CurrentImagesCount { get; set; }

            [DataMember]
            [JsonProperty(PropertyName = "platesCount")]
            public long CurrentPlatesCount { get; set; }


            [DataMember]
            [JsonProperty(PropertyName = "imagesLimit")]
            public long ImagesLimit { get; set; }

            [DataMember]
            [JsonProperty(PropertyName = "platesLimit")]
            public long PlatesLimit { get; set; }


        }

        /// <summary>
        /// Checks last session by userId and update it if expiried already, then creates new session
        /// </summary>
        /// <param name="userId"></param>
        public void CheckExpirySessionByUserId(long userId)
        {
            using(var dbContext = new ApiDbContext())
            {
                var userByUserId = dbContext.Set<User>().Where(p => p.Id == userId).ToList().LastOrDefault();
                var sessionByUserId = dbContext.Set<Session>().Where(p => p.UserId == userId).ToList().LastOrDefault();

                if(sessionByUserId.ExpiryDate < DateTime.Now)
                {
                    var newSession = new Session
                    {
                        CreationTime = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddMonths(1),
                        ImageCount = 0,
                        IsLimitReached = false,
                        PlatesCount = 0,
                        SessionType = userByUserId.UserType,
                        UserId = userByUserId.Id
                    };

                    dbContext.Sessions.Add(newSession);
                    dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Checks last session by userId on detection limits and sets IsLimitReached on current session
        /// in case of limit is reached already
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true : if at least one of limits is reached</returns>
        //public bool CheckLimitByUserId(long userId)
        //{
        //    using (var dbContext = new ApiDbContext())
        //    {
        //        var sessionByUserId = dbContext.Set<Session>().Where(p => p.UserId == userId).ToList().LastOrDefault();

        //        if (sessionByUserId.IsLimitReached == true)
        //        {
        //            return true;
        //        }

        //        if (sessionByUserId.PlatesCount >= LimitValues.PlatesCountLimit || sessionByUserId.ImageCount >= LimitValues.ImageCountLimit)
        //        {
        //            sessionByUserId.IsLimitReached = true;
        //            dbContext.SaveChanges();
        //            return true;
        //        }

        //        return false;
        //    }
        //}
        

        /// <summary>
        /// Check session on limits and returns
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public AvailableLimits CheckLimitByUserId(long userId)
        {
            var availableLimits = new AvailableLimits
            {
                ImagesLimit = LimitValues.ImageCountLimit,
                PlatesLimit = LimitValues.PlatesCountLimit,
                IsLimitReached = false
            };

            using (var dbContext = new ApiDbContext())
            {
                var sessionByUserId = dbContext.Set<Session>().Where(p => p.UserId == userId).ToList().LastOrDefault();

                availableLimits.CurrentImagesCount = sessionByUserId.ImageCount;
                availableLimits.CurrentPlatesCount = sessionByUserId.PlatesCount;
                availableLimits.IsLimitReached = true;

                if (sessionByUserId.IsLimitReached == true)
                {
                    return availableLimits;
                }

                if (sessionByUserId.PlatesCount >= LimitValues.PlatesCountLimit || sessionByUserId.ImageCount >= LimitValues.ImageCountLimit)
                {
                    sessionByUserId.IsLimitReached = true;
                    dbContext.SaveChanges();

                    return availableLimits;
                }

                return availableLimits;
            }
        }
    }
}
