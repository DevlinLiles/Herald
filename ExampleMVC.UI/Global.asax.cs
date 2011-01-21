using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace ExampleMVC.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : JumpStart.JumpStartApplication
    {
        protected override void RegisterRoutes(RouteCollection routes)
        {
            base.RegisterRoutes(routes);

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected override void RegisterServices(IWindsorContainer container)
        {
            container.Install(FromAssembly.Containing<ControllersInstaller>());
            base.RegisterServices(container);
        }

        protected override void OnApplicationStart()
        {
            base.OnApplicationStart();
        }

        protected override void OnApplicationEnd()
        {
            base.OnApplicationEnd();
        }
    }
}