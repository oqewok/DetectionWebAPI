using DetectionAPI.Database;
using DetectionAPI.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using DetectionAPI.Database.Entities;

namespace DetectionAPI.Filters
{
    public class RealBasicAuthenticationFilter : BasicAuthenticationFilter
    {

        public RealBasicAuthenticationFilter()
        {

        }

        public RealBasicAuthenticationFilter(bool active) : base(active)
        {

        }


        protected override bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {

            // some logic here to look at if user exists
            // now there's just a stub, that creates it on code running

            using(var dbContext = new ApiDbContext())
            {
                var user = dbContext.Set<User>().Where(p => p.Username == username).Where(p => p.Password == password).ToList().FirstOrDefault();

                if (user == null)
                {
                    return false;
                }

                return true;
            }

            //var apiUser = new ApiUser()
            //{
            //    Id = 1,
            //    Name = "apiUser",
            //    Username = "api_login",
            //    Password = "api_password1234"
            //};


            //if (apiUser == null)
            //    return false;

            //if (apiUser.Username != username || apiUser.Password != password)
            //{
            //    return false;
            //}

            //return true;
        }
    }
}
