using System;
using System.Web.Mvc;
using Herald.Core.ViewModels;
using Herald.Infrastructure.Repositories;
using Herald.UI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herald.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            var controller = new HomeController(new InMemoryRepository(), null);

            // Act
            var model = controller.Index().As<ViewResult>().ViewData.Model;

            // Assert
            Assert.IsTrue(model is HomeIndexModel);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            var controller = new HomeController(new InMemoryRepository(), null);

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}