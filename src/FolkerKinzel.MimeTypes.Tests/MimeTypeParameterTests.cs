using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeTypeParameterTests
{
    [TestMethod]
    public void CloneTest1()
    {
        Assert.IsTrue(MimeType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeType inetMedia));
        ICloneable cloneable = inetMedia.Parameters().First();
        object o = cloneable.Clone();
        Assert.AreEqual(cloneable, o);
        Assert.AreEqual("charset=iso-8859-1", o.ToString());
    }

    [TestMethod]
    public void EqualsTest1()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=US-ASCII", out MimeType media1));
        object o1 = media1.Parameters().First();
        Assert.IsFalse(o1.Equals(42));
    }

    [TestMethod]
    public void ParseTest1()
    {
        string longMessage = """
         message / external-body ; (Comment)
        URL = "ftp://cs.utk.edu/pub/moore/bulk-mailer/bulk-mailer.tar" (Comment)
        """;

        var mime = MimeType.Parse(longMessage);
        Assert.AreEqual(1, mime.Parameters().Count());

        //foreach (MimeTypeParameter param in mime.Parameters()) 
        //{
        //    ReadOnlySpan<char> key = param.Key;
        //    ReadOnlySpan<char> value = param.Value;
        //}

        //string s = mime.ToString();
    }

    [TestMethod]
    public void TryParseTest1()
    {
        ReadOnlyMemory<char> mem = "key:value".AsMemory();
        Assert.IsFalse(MimeTypeParameter.TryParse(true, ref mem, out _, out _));
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AppendToTest1() => MimeTypeParameter.Empty.AppendTo(null!);


    [TestMethod]
    public void AppendToTest2()
    {
        var sb = new StringBuilder();
        MimeTypeParameter.Empty.AppendTo(sb);
        Assert.AreEqual(0, sb.Length);
    }


    [TestMethod]
    public void CharsetTest1()
    {
        string input = "key*=utf-8'de'" + Uri.EscapeDataString("äääääääääh");
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameter.TryParse(true, ref mem, out MimeTypeParameter parameter, out _));
        Assert.AreEqual("utf-8", parameter.CharSet.ToString());
        Assert.AreEqual("de", parameter.Language.ToString());

    }




}
