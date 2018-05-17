using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DetectionAPI.Infrastructure;
using DetectionAPIWebHost;
using Ninject;
using Owin;

namespace DetectionAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //IKernel kernel = new StandardKernel();
            //var dr = new NinjectDependencyResolver(kernel);
            //GlobalConfiguration.Configuration.DependencyResolver = dr;

            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);
        }
    }
}
