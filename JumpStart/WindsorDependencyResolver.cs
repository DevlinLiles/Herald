using System;
using Castle.Windsor;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JumpStart
{
    public class WindsorDependencyResolver : IDependencyResolver
    {
        private readonly IWindsorContainer container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if (container.Kernel.HasComponent(serviceType))
                return container.Resolve(serviceType);
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (container.Kernel.HasComponent(serviceType))
                return (object[])container.ResolveAll(serviceType);
            return new object[] { };
        }
    }
}
