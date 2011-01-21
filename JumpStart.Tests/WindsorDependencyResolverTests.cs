using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using System.IO;

namespace JumpStart.Tests
{
    [TestClass]
    public class WindsorDependencyResolverTests : BaseTest
    {
        private WindsorDependencyResolver resolver;

        public override void RegisterServices(IWindsorContainer container)
        {
            resolver = new WindsorDependencyResolver(container);
            base.RegisterServices(container);
        }
        
        [TestMethod]
        public void Should_GetService_If_Type_Registered()
        {
            container.Register(Component.For<IDisposable>().UsingFactoryMethod(() => new StringReader("foo")));

            var resolution = resolver.GetService(typeof(IDisposable));

            Assert.IsInstanceOfType(resolution, typeof(StringReader));
        }

        [TestMethod]
        public void Should_GetService_Null_If_Unregistered()
        {
            var resolution = resolver.GetService(typeof(IDisposable));

            Assert.IsNull(resolution);
        }

        [TestMethod]
        public void Should_GetServices_If_Type_Registered()
        {
            container.Register(Component.For<IDisposable>().UsingFactoryMethod(() => new StringReader("foo")).Named("foo"));
            container.Register(Component.For<IDisposable>().UsingFactoryMethod(() => new StringReader("bar")).Named("bar"));
            container.Register(Component.For<IDisposable>().UsingFactoryMethod(() => new StringReader("far")).Named("far"));

            var resolution = resolver.GetServices(typeof(IDisposable));

            Assert.AreEqual(3, resolution.Count());
            foreach (var comp in resolution)
            {
                Assert.IsInstanceOfType(comp, typeof(StringReader));
            }
        }

        [TestMethod]
        public void Should_GetServices_If_Type_Unregistered()
        {
            var resolution = resolver.GetServices(typeof(IDisposable));

            Assert.AreEqual(0, resolution.Count());
        }
    }
}
