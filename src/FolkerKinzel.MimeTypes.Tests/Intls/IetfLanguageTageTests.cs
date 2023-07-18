using FolkerKinzel.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class IetfLanguageTagTests
{
    [TestMethod]
    public void ValidateTest1() => Assert.IsFalse(IetfLanguageTag.Validate("   "));
}


