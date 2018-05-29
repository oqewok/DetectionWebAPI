using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;
using System.Threading;

using DetectionAPI.Database;
using DetectionAPI.Database.Entities;
using DetectionAPI.Filters;
using DetectionAPI.Helpers;
using Newtonsoft.Json;

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

            long userId = -1;

            using (var dbContext = new ApiDbContext())
            {
                var certainUser = dbContext.Set<User>().Where(p => p.Username == name).ToList().FirstOrDefault();

                if(certainUser != null)
                {
                    userId = certainUser.Id;
                    message = "token";
                    token = certainUser.AccessToken;
                }
            }

            //Check if last session is expired and create new in that case
            if (userId != -1)
            {
                CheckHelper.CheckExpirySessionByUserId(userId);
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

            long userId = -1;

            using (var dbContext = new ApiDbContext())
            {
                var certainUser = dbContext.Set<User>().Where(p => p.Username == username).ToList().FirstOrDefault();

                if (certainUser != null)
                {
                    userId = certainUser.Id;
                    message = "token";
                    token = Guid.NewGuid().ToString("N");
                    certainUser.AccessToken = token;
                    dbContext.SaveChanges();
                }
            }

            //Check if last session is expired and create new in that case
            if (userId != -1)
            {
                CheckHelper.CheckExpirySessionByUserId(userId);
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
            long userId = -1;

            using (var dbContext = new ApiDbContext())
            {
                var user = dbContext.Set<User>().Where(p => p.AccessToken == authorizedUserToken).ToList().LastOrDefault();

                if (user != null)
                {
                    userId = user.Id;
                }
            }

            if (userId != -1)
            {
                var currentLimitInfo = CheckHelper.CheckLimitByUserId(userId);
                return Ok(currentLimitInfo);
            }

            else
            {
                return BadRequest();
            }  
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
