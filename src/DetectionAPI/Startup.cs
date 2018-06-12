using System.Web.Http;
using DetectionAPIWebHost;
using Owin;

namespace DetectionAPI
{
    public class Startup
    {
        /// <summary>
        /// Конифгурирует приложение, сопоставляет контроллеры, их
        /// методы с URI маршрутов
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);
        }
    }
}
