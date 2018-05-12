﻿using DetectionAPI.Detection;
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
            _kernel.Bind<IDetector>().To<FakeDetector>().InSingletonScope().WithPropertyValue("Name", Guid.NewGuid());
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