using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Castle.Windsor;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using System.Web.Mvc;
using Castle.Core.Logging;
using Castle.Services.Logging.Log4netIntegration;

namespace JumpStart.Tests
{

    [TestClass]
    public class BaseControllerTests : BaseTest
    {
        public override void RegisterServices(IWindsorContainer container)
        {
            container.Register(Component.For<ILoggingController>().ImplementedBy<MockController>());
            base.RegisterServices(container);
        }

        [TestMethod]
        public void Should_Populate_The_Logger()
        {
            var cntrl = container.Resolve<ILoggingController>();

            Assert.IsInstanceOfType(cntrl, typeof(MockController));
            Assert.IsInstanceOfType(cntrl.Logger, typeof(Log4netLogger));
        }
    }

    public class MockController : BaseController
    {
    }
}
