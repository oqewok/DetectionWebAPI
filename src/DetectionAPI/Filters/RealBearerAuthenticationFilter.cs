using DetectionAPI.Database;
using System.Linq;
using System.Web.Http.Controllers;
using DetectionAPI.Database.Entities;

namespace DetectionAPI.Filters
{
    /// <summary>
    /// Класс, переопределяющий поведение базового фильтра авторизации типа Bearer
    /// </summary>
    public class RealBearerAuthenticationFilter : BearerAuthenticationFilter
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public RealBearerAuthenticationFilter()
        {

        }

        /// <summary>
        /// Конструктор с указанием активности данного фильтра
        /// </summary>
        public RealBearerAuthenticationFilter(bool active) : base(active)
        {

        }

        /// <summary>
        /// Соверщает запрос к базе данных с целью установления существования пользователя, его личности и прав доступа
        /// </summary>
        /// <param name="token">Переданный с запросом токен доступа</param>
        /// <param name="actionContext">Контекст выполнения запроса</param>
        /// <returns>Статус прохождения авторизации</returns>
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
