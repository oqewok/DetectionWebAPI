using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAPI.Filters
{
    public class BearerAuthenticationIdentity : GenericIdentity
    {
        public BearerAuthenticationIdentity(string token) : base(token, "Bearer")
        {

        }
    }
}
