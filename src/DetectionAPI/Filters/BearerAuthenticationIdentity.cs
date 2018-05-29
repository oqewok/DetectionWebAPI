using System.Security.Principal;

namespace DetectionAPI.Filters
{
    public class BearerAuthenticationIdentity : GenericIdentity
    {
        public BearerAuthenticationIdentity(string token) : base(token, "Bearer")
        {

        }
    }
}
