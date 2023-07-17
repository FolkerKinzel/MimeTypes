using FolkerKinzel.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class MimeTypeParameterBuilderTests
{
    [TestMethod]
    public void BuildTest1()
    {
        var sb = new StringBuilder();
        const string nonAscii = "ä";
        const string ascii = "para";

        ParameterSerializer.Append(sb, new MimeTypeParameterModel(ascii, nonAscii));
        Assert.AreNotEqual(0, sb.Length);
        string s = sb.ToString();
        Assert.IsTrue(s.Contains(ascii));
        Assert.IsFalse(s.Contains(nonAscii));
    }
}
