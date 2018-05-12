using DetectionAPI.Detection;
using DetectionAPI.Infrastructure;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using Ninject.Web.WebApi;
using Ninject;

namespace DetectionAPIWebHost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API configuration and services

            //Removes XML formatter and sets new Newtonsoft.Json.Serializer as default
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

            //PlateDetector.Detection.Detector MainDetector = new PlateDetector.Detection.Detector(new PlateDetector.Detection.AlgManager(new PlateDetector.Detection.FasterRcnnProvider
            //config.Services.Add(typeof(PlateDetector.Detection.Detector), MainDetector);

            //FakeDetector fd = new FakeDetector();

            //config.Services.Add(typeof(FakeDetector), fd);




            //config.DependencyResolver = new DetectionAPI.Infrastructure.NinjectDependencyResolver();

            IKernel kernel = new StandardKernel();

            config.DependencyResolver = new DetectionAPI.Infrastructure.NinjectDependencyResolver(kernel);



            //Override when the error detail gets sent back to clients directly
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller="Home", id = RouteParameter.Optional }
            );


        }
    }
}
