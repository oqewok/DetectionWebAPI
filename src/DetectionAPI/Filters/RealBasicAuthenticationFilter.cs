using DetectionAPI.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

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

            var apiUser = new ApiUser()
            {
                Id = 1,
                Name = "apiUser",
                Username = "api_login",
                Password = "api_password1234"
            };


            if (apiUser == null)
                return false;

            if (apiUser.Username != username || apiUser.Password != password)
            {
                return false;
            }

            return true;
        }
    }
}
