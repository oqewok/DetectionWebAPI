using System.Security.Principal;

namespace DetectionAPI.Filters
{
    /// <summary>
    /// /// Описывает данные авторизации для типа авториции Basic
    /// </summary>
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string name, string password)
            : base(name, "Basic")
        {
            this.Password = password;
        }

        /// <summary>
        /// Пароль, переданный с запросом
        /// </summary>
        public string Password { get; set; }
    }
}
