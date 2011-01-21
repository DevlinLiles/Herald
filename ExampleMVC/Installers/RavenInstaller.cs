using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using Raven.Client;
using Raven.Client.Client;
using System.IO;

namespace ExampleMVC
{
    public class RavenInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IDocumentStore>()
                                .UsingFactoryMethod(() => GetDocumentStore())
                                .LifeStyle.Singleton);
        }

        private IDocumentStore GetDocumentStore()
        {
            var documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\RavenDB"),
                UseEmbeddedHttpServer = true
            };
            documentStore.Initialize();
            return documentStore;
        }
    }
}
