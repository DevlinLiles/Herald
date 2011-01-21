using System;
using System.Web.Mvc;
using Castle.MicroKernel;
using System.Web.Routing;

namespace JumpStart
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            kernel.ReleaseComponent(controller);
        }

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var name = controllerName + "Controller";
            return kernel.Resolve<IController>(name);
        }
    }
}
