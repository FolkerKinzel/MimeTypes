using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Tests
{
    [TestClass]
    public class MimeTypeParameterTests
    {
        [TestMethod]
        public void CloneTest1()
        {
            Assert.IsTrue(MimeType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeType inetMedia));
            ICloneable cloneable = inetMedia.Parameters.First();
            object o = cloneable.Clone();
            Assert.AreEqual(cloneable, o);
            Assert.AreEqual("charset=iso-8859-1", o.ToString());
        }

        [TestMethod]
        public void EqualsTest1()
        {
            Assert.IsTrue(MimeType.TryParse("text/plain; charset=US-ASCII", out MimeType media1));
            object o1 = media1.Parameters.First();
            Assert.IsFalse(o1.Equals(42));
        }
    }
}
