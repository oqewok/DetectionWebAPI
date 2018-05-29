using DetectionAPI.Database;
using System.Linq;
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
