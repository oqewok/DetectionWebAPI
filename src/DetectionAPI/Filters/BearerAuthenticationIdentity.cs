using System.Security.Principal;

namespace DetectionAPI.Filters
{
    /// <summary>
    /// Описывает данные авторизации для типа авториции Bearer
    /// </summary>
    public class BearerAuthenticationIdentity : GenericIdentity
    {
        public BearerAuthenticationIdentity(string token) : base(token, "Bearer")
        {

        }
    }
}
