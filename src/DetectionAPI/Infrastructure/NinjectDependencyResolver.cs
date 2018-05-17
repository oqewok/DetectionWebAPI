using DetectionAPI.Detection;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using Ninject.Extensions;
using Ninject.Syntax;
using Ninject.Activation;
using Ninject.Parameters;
using PlateDetector.Detection;
using DetectionAPI.Controllers;
using System.Web.Http.Controllers;

namespace DetectionAPI.Infrastructure
{
    public class NinjectDependencyResolver : NinjectScope, IDependencyResolver
    {
        private IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel) : base(kernel)
        {
            _kernel = kernel;
            AddBindings();
            
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectScope(_kernel.BeginBlock());
        }

        private void AddBindings()
        {
            //_kernel.Bind<IDetector>().To<FakeDetector>().InSingletonScope();

            //works!
            //_kernel.Bind<FakeDetector>().ToSelf().InSingletonScope();

            ////works!
            //_kernel.Bind<FakeDetector.Insiderinsider>().ToSelf().WithPropertyValue("InsiderInsiderName", Guid.NewGuid());
            //_kernel.Bind<FakeDetector.FakeDetectorInsider>().ToSelf();

            ////_kernel.Bind<FakeDetector>().ToSelf()
            ////    .InSingletonScope()
            ////    .WithConstructorArgument("FakeDetectorInsider", new FakeDetector.FakeDetectorInsider(new FakeDetector.Insiderinsider()));
            //_kernel.Bind<FakeDetector>().ToSelf().InSingletonScope();
            ////

            //_kernel.Bind<IDetectionAlgProvider>().To<FasterRcnnProvider>().InSingletonScope();
            ////_kernel.Bind<FasterRcnn>().ToSelf();
            //_kernel.Bind<IDetectionAlg>().To<FasterRcnn>().InSingletonScope();
            ////_kernel.Bind<FasterRcnnProvider>().ToSelf();
            //_kernel.Bind<AlgManager>().ToSelf().InSingletonScope();
            //_kernel.Bind<Detector>().ToSelf().InSingletonScope();


            //_kernel.Bind<FasterRcnnProvider>().ToSelf().InSingletonScope();
            //_kernel.Bind<AlgManager>().ToSelf().InSingletonScope().WithConstructorArgument("FasterRcnnProvider", new FasterRcnnProvider());
            //_kernel.Bind<Detector>().ToSelf().InSingletonScope().WithConstructorArgument("AlgManager", new AlgManager());

            //_kernel.Bind<FasterRcnnProvider>().ToSelf().InSingletonScope();
            //_kernel.Bind<AlgManager>().ToSelf().InSingletonScope();

            //_kernel.Bind<IAlg>
            //_kernel.Bind<Detector>().ToSelf().InSingletonScope();


            //_kernel.Bind<Detector>()
            //    .ToSelf().
            //    InSingletonScope().
            //    WithConstructorArgument("AlgManager", new AlgManager(new FasterRcnnProvider()));

            //_kernel.Bind<IHttpController>().To<DetectController>();
            //_kernel.Bind<DetectController>().ToSelf().InRequestScope();

            _kernel.Bind<IHttpController>().To<ABCController>().InRequestScope();

            _kernel.Bind<IDetectionAlgProvider>().To<FasterRcnnProvider>().InSingletonScope();
            _kernel.Bind<FasterRcnn>().ToSelf();
            _kernel.Bind<IDetectionAlg>().To<FasterRcnn>().InSingletonScope();
            _kernel.Bind<FasterRcnnProvider>().ToSelf();
            _kernel.Bind<AlgManager>().ToSelf().InSingletonScope();
            _kernel.Bind<Detector>().ToSelf().InSingletonScope();
            _kernel.Bind<IDetector>().To<FakeDetector>().InSingletonScope();
        }

    }


    public class NinjectScope : IDependencyScope
    {
        protected IResolutionRoot resolutionRoot;

        public NinjectScope(IResolutionRoot kernel)
        {
            resolutionRoot = kernel;
        }

        public object GetService(Type serviceType)
        {
            IRequest request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
            return resolutionRoot.Resolve(request).SingleOrDefault();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            IRequest request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
            return resolutionRoot.Resolve(request).ToList();
        }

        public void Dispose()
        {
            IDisposable disposable = (IDisposable)resolutionRoot;
            if (disposable != null) disposable.Dispose();
            resolutionRoot = null;
        }
    }


}
