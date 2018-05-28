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
    public class RealBearerAuthenticationFilter : BearerAuthenticationFilter
    {
        public RealBearerAuthenticationFilter()
        {

        }

        public RealBearerAuthenticationFilter(bool active) : base(active)
        {

        }

        protected override bool OnAuthorizeUser(string token, HttpActionContext actionContext)
        {

            // some logic here to look at if user exists
            // now there's just a stub, that creates it on code running

            using (var dbContext = new ApiDbContext())
            {
                var user = dbContext.Set<User>().Where(p => p.AccessToken == token).ToList().FirstOrDefault();

                if (user == null)
                {
                    return false;
                }

                return true;
            }

        }


    }
}
