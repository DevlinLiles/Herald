using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JumpStart.Tests
{
    [TestClass]
    public class IsNotTests : BaseTest
    {
        private IsNot isNot;

        public override void BeforeEachTest()
        {
            isNot = new IsNot();
            base.BeforeEachTest();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Null_Throws_When_Null()
        {
            IDisposable val = null;

            isNot.Null(val, "password");
        }

        [TestMethod]
        public void Null_ReturnsIsNot_When_NotNull()
        {
            var val = "";

            var result = isNot.Null(val, "password");

            Assert.IsInstanceOfType(result, typeof(IsNot));
            Assert.AreSame(isNot, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullOrEmpty_Throws_When_Null()
        {
            string val = null;

            isNot.NullOrEmpty(val, "password");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullOrEmpty_Throws_When_Empty()
        {
            string val = "";

            isNot.NullOrEmpty(val, "password");
        }

        [TestMethod]
        public void NullOrEmpty_ReturnsIsNot_When_NotNull()
        {
            var val = "foo";

            var result = isNot.NullOrEmpty(val, "password");

            Assert.IsInstanceOfType(result, typeof(IsNot));
            Assert.AreSame(isNot, result);
        }
    }
}
