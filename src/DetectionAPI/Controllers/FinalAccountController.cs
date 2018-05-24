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

namespace DetectionAPI.Controllers
{
    public class FinalAccountController : ApiController
    {
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

        public IHttpActionResult Token()
        {
            return NotFound();
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
