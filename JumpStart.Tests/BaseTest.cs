using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Castle.Windsor;
using Castle.Facilities.Logging;
using Castle.Core.Logging;

namespace JumpStart.Tests
{
    [TestClass]
    public class BaseTest
    {
        protected IWindsorContainer container;
        protected ILogger logger;

        [TestInitialize]
        public void TestInitialize()
        {
            container = new WindsorContainer();
            RegisterServices(container);
            logger = container.Resolve<ILoggerFactory>().Create(this.GetType());

            logger.Debug("Begin BeforeEachTest()");
            BeforeEachTest();
            logger.Debug("Begin [TestMethod]");
        }

        public virtual void RegisterServices(IWindsorContainer container)
        {
            container.AddFacility<LoggingFacility>(c => c.LogUsing(LoggerImplementation.Log4net));
        }

        public virtual void BeforeEachTest()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
            logger.Debug("End [TestMethod]");
            AfterEachTest();
            logger.Debug("End AfterEachTest()");
        }

        public virtual void AfterEachTest()
        {
        }
    }
}
