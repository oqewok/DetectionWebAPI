using DetectionAPI.Database;
using System.Linq;
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
            using(var dbContext = new ApiDbContext())
            {
                var user = dbContext.Set<User>().Where(p => p.Username == username).Where(p => p.Password == password).ToList().FirstOrDefault();

                if (user == null)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
