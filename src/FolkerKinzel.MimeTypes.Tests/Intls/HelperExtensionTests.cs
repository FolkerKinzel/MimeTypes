using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls.Tests
{
    [TestClass]
    public class HelperExtensionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToLowerInvariantTest1()
        {
            StringBuilder? sb = null;
            _ = sb!.ToLowerInvariant();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLowerInvariantTest2()
        {
            var sb = new StringBuilder();
            _ = sb.ToLowerInvariant(-15);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLowerInvariantTest3()
        {
            var sb = new StringBuilder();
            _ = sb.ToLowerInvariant(0,4711);
        }

        [TestMethod]
        public void ToLowerInvariantTest4()
        {
            var sb = new StringBuilder("TEST");
            Assert.AreEqual("test", sb.ToLowerInvariant().ToString());
        }

        [TestMethod]
        public void ToLowerInvariantTest12()
        {
            var sb = new StringBuilder("TEST");
            Assert.AreEqual("Test", sb.ToLowerInvariant(1).ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToLowerInvariantTest5()
        {
            StringBuilder? sb = null;
            _ = sb!.ToLowerInvariant(17);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToLowerInvariantTest6()
        {
            StringBuilder? sb = null;
            _ = sb!.ToLowerInvariant(17, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLowerInvariantTest7()
        {
            var sb = new StringBuilder();
            _ = sb.ToLowerInvariant(0,-4711);
        }

        [TestMethod]
        public void ToLowerInvariantTest8()
        {
            var sb = new StringBuilder();
            _ = sb.ToLowerInvariant(0,0);
        }

        [TestMethod]
        public void ToLowerInvariantTest9()
        {
            var sb = new StringBuilder();
            _ = sb.ToLowerInvariant(0);
        }

        [TestMethod]
        public void ToLowerInvariantTest10()
        {
            var sb = new StringBuilder();
            _ = sb.ToLowerInvariant();
        }

        [DataTestMethod]
        [DataRow("   ")]
        [DataRow("")]
        public void GetTrimmedStartTest1(string s) => Assert.AreEqual(s.Length, s.AsSpan().GetTrimmedStart());
    }
}
