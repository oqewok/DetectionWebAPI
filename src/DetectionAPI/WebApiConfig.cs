using Newtonsoft.Json.Serialization;
using System.Web.Http;
using Ninject;

namespace DetectionAPIWebHost
{
    public static class WebApiConfig
    {
        // Конфигурация Web API и всех сервисов приложения
        public static void Register(HttpConfiguration config)
        {
            //Удалим XML форматировщики вывода и установим новый для формата JSON
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

            IKernel kernel = new StandardKernel();
            config.DependencyResolver = new DetectionAPI.Infrastructure.NinjectDependencyResolver(kernel);

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

            //Конифгурация стандартного маршрута, остальные маршруты динамически строятся на основании атрибутов к классам контроллеров
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { }
            );
        }
    }
}
