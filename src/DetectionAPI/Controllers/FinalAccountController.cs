using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Threading;

using DetectionAPI.Database;
using DetectionAPI.Database.Entities;
using DetectionAPI.Filters;
using DetectionAPI.Helpers;
using Newtonsoft.Json;

namespace DetectionAPI.Controllers
{
    /// <summary>
    /// Класс, принимающий запросы удаленных пользователей и осуществляющий действия
    /// по созданию пользователей и контролю их сессий, осуществляющий генерацию и
    /// выдачу токенов доступа
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FinalAccountController : ApiController
    {
        /// <summary>
        /// Создает нового пользователя с именем и паролем, переданными в теле
        /// отправленного запроса, который передается в виде текста формата application/json
        /// </summary>
        /// <returns>HTTP-коды 200 OK или 400 BadRequest</returns>
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

                //После проверок создается новый пользователь
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
        /// Возвращает для авторизированного пользователя токен доступа 
        /// </summary>
        /// <returns>HTTP-коды 200 OK стокеном доступа в теле ответа или 400 BadRequest</returns>
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

            //Проверяет последнюю сессию и если она истекла, создает новую
            if (userId != -1)
            {
                CheckHelper.CheckExpirySessionByUserId(userId);
            }

            tokenDict.Add(message, token);
            return CreatedAtRoute(routeName: "GetAccessTokenHeaderParameter", routeValues: new {}, content: tokenDict);
        }

        /// <summary>
        /// Обновляет токен доступа и отправляет его удаленному пользователю
        /// </summary>
        /// <returns>HTTP-коды 200 OK с токеном доступа в теле ответа или 400 BadRequest</returns>
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

            //Проверяет последнюю сессию и если она истекла, создает новую
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

        /// <summary>
        /// Проверяет значения ограничений использования для прошедшего авторизацию пользователя
        /// </summary>
        /// <returns>HTTP-коды 200 OK со значениями ограничений для текущей сессии в теле ответа или 400 BadRequest</returns>
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

        /// <summary>
        /// Данные, передаваемые в теле запроса для создания пользователя
        /// </summary>
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
