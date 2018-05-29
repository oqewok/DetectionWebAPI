using Newtonsoft.Json.Serialization;
using System.Web.Http;
using Ninject;

namespace DetectionAPIWebHost
{
    public static class WebApiConfig
    {
        // Web API configuration and services
        public static void Register(HttpConfiguration config)
        {
            //Removes XML formatter and sets new Newtonsoft.Json.Serializer as default
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

            IKernel kernel = new StandardKernel();
            config.DependencyResolver = new DetectionAPI.Infrastructure.NinjectDependencyResolver(kernel);

            //Override when the error detail gets sent back to clients directly
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { }
            );
        }
    }
}
