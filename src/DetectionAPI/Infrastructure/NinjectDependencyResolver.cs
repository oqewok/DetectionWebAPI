using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Ninject.Syntax;
using Ninject.Activation;
using Ninject.Parameters;
using PlateDetector.Detection;

namespace DetectionAPI.Infrastructure
{
    /// <summary>
    /// Класс, описывающий и регистрирующий зависимости объектов, управляющий временем их жизни
    /// </summary>
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

        /// <summary>
        /// Регистрация зависимостей
        /// </summary>
        private void AddBindings()
        {
            _kernel.Bind<IDetectionAlgProvider>().To<FasterRcnnProvider>().InSingletonScope();
            _kernel.Bind<FasterRcnn>().ToSelf();
            _kernel.Bind<IDetectionAlg>().To<FasterRcnn>().InSingletonScope();
            _kernel.Bind<FasterRcnnProvider>().ToSelf();
            _kernel.Bind<AlgManager>().ToSelf().InSingletonScope();
            _kernel.Bind<Detector>().ToSelf().InSingletonScope();
            _kernel.Bind<IDetector>().To<Detector>().InSingletonScope();
        }

    }

    /// <summary>
    /// Управляет передачей объектов по месту использования
    /// </summary>
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
