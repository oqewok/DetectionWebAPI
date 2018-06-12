using DetectionAPI.Database;
using System.Linq;
using System.Web.Http.Controllers;
using DetectionAPI.Database.Entities;

namespace DetectionAPI.Filters
{
    /// <summary>
    /// Класс, переопределяющий поведение базового фильтра авторизации типа Basic
    /// </summary>
    public class RealBasicAuthenticationFilter : BasicAuthenticationFilter
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public RealBasicAuthenticationFilter()
        {

        }

        /// <summary>
        /// Конструктор с указанием активности данного фильтра
        /// </summary>
        public RealBasicAuthenticationFilter(bool active) : base(active)
        {

        }

        /// <summary>
        /// Соверщает запрос к базе данных с целью установления существования пользователя, его личности и прав доступа
        /// </summary>
        /// <param name="username">Переданное с запросом имя пользователя</param>
        /// <param name="password">Переданный с запросом пароль</param>
        /// <param name="actionContext">Контекст выполнения запроса</param>
        /// <returns>Статус прохождения авторизации</returns>
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
