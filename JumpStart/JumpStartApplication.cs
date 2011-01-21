using System;
using System.Web.Mvc;
using Castle.Windsor;
using System.Web.Routing;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Facilities.Logging;
using Castle.Core.Logging;

namespace JumpStart
{
    public class JumpStartApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;
        protected virtual void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
        }

        protected virtual void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Start()
        {
            OnApplicationStart();
        }

        protected void Application_End()
        {
            OnApplicationEnd();
        }

        protected virtual void OnApplicationStart()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BuildContainer();
        }

        protected virtual void OnApplicationEnd()
        {
            container.Dispose();
        }

        protected virtual void RegisterServices(IWindsorContainer container)
        {
            container.AddFacility<LoggingFacility>(c => c.LogUsing(LoggerImplementation.Log4net));
        }

        private void BuildContainer()
        {
            container = new WindsorContainer();
            container.Register(Component.For<IControllerFactory>().ImplementedBy<WindsorControllerFactory>());

            RegisterServices(container);
            DependencyResolver.SetResolver(new WindsorDependencyResolver(container));
        }
    }
}
