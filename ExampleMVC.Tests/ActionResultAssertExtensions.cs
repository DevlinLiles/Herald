using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace ExampleMVC.Tests
{
    public static class ActionResultAssertExtensions
    {
        public static ViewResult AssertIsAView(this ActionResult result)
        {
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            return result as ViewResult;
        }
        public static ViewResult AssertIsAViewOf<T>(this ActionResult result)
        {
            var view = result.AssertIsAView();
            Assert.IsInstanceOfType(view.Model, typeof(T));
            return view;
        }
    }
}
