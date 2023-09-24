using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeTypeParameterInfoTests
{
    [TestMethod]
    public void CloneTest1()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeTypeInfo inetMedia));
        ICloneable cloneable = inetMedia.Parameters().First();
        object o = cloneable.Clone();
        Assert.AreEqual(cloneable, o);
    }

    [TestMethod]
    public void CloneTest2()
    {
        var para = new MimeTypeParameterInfo();
        MimeTypeParameterInfo clone = para.Clone();
        Assert.AreEqual(para, clone);
    }

        [TestMethod]
    public void EqualsTest1()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=US-ASCII", out MimeTypeInfo media1));
        object o1 = media1.Parameters().First();
        Assert.IsFalse(o1.Equals(42));
    }

    [TestMethod]
    public void EqualsTest2()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=US-ASCII", out MimeTypeInfo media1));
        MimeTypeParameterInfo o1 = media1.Parameters().First();
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=us-ascii", out MimeTypeInfo media2));
        MimeTypeParameterInfo o2 = media2.Parameters().First();
        Assert.IsTrue(o1.Equals(o2));
        Assert.IsTrue(o1 == o2);
        Assert.IsFalse(o1 != o2);
    }

    [TestMethod]
    public void EqualsTest3()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=US-ASCII", out MimeTypeInfo media1));
        MimeTypeParameterInfo o1 = media1.Parameters().First();
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; other=US-ASCII", out MimeTypeInfo media2));
        MimeTypeParameterInfo o2 = media2.Parameters().First();
        Assert.IsFalse(o1.Equals(o2));
        Assert.IsTrue(o1 != o2);
        Assert.IsFalse(o1 == o2);
    }

    [TestMethod]
    public void ParseTest1()
    {
        string longMessage = """
         message / external-body ; (Comment)
        URL = "ftp://cs.utk.edu/pub/moore/bulk-mailer/bulk-mailer.tar" (Comment)
        """;

        var mime = MimeTypeInfo.Parse(longMessage);
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
        Assert.IsFalse(MimeTypeParameterInfo.TryParse(true, ref mem, out _, out _));
    }

    [TestMethod]
    public void TryParseTest2()
    {
        string lang = new('a', 300);
        ReadOnlyMemory<char> mem = $"key*=utf-8'{lang}'value".AsMemory();
        Assert.IsFalse(MimeTypeParameterInfo.TryParse(true, ref mem, out _, out _));
    }

    [TestMethod]
    public void TryParseTest3()
    {
        ReadOnlyMemory<char> mem = "key*=utf-8'en'%EF%AB%CD".AsMemory();
        Assert.IsFalse(MimeTypeParameterInfo.TryParse(true, ref mem, out _, out _));
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AppendToTest1() => new MimeTypeParameter("k", null, null).AppendTo(null!);


    [TestMethod]
    public void AppendToTest2()
    {
        string s = MimeTypeParameterInfo.Empty.ToString();
        Assert.IsNotNull(s);
        Assert.AreEqual(0, s.Length);
    }

    [TestMethod]
    public void AppendToTest3()
    {
        const string input = "x/y; normal=value";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeType.TryParse(mem, out MimeType? mime));
        var sb = new StringBuilder();
        _ = mime.AppendTo(sb, MimeFormats.Url);
        Assert.AreEqual("x/y;normal=value", sb.ToString());
    }

    [TestMethod]
    public void AppendToTest4()
    {
        const string input = "x/y; quoted=\"text loch\"";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeType.TryParse(mem, out MimeType? mime));
        var sb = new StringBuilder();
        _ = mime.AppendTo(sb, MimeFormats.Url);
        Assert.AreEqual("x/y;quoted*=utf-8''text%20loch", sb.ToString());
    }

    [TestMethod]
    public void AppendToTest5()
    {
        const string input = "y/y; encoded*=utf-8''value%20continuation";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeType.TryParse(mem, out MimeType? mime));
        Assert.AreEqual("y/y;encoded*=utf-8''value%20continuation", mime.ToString(MimeFormats.Url));
    }


    [TestMethod]
    public void ToStringTest1()
    {
        const string input = "x/y; normal=value";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeType.TryParse(mem, out MimeType? mime));
        Assert.AreEqual("x/y;normal=value", mime.ToString(MimeFormats.Url));
    }

    [TestMethod]
    public void ToStringTest2()
    {
        const string input = "x/y; quoted=\"text loch\"";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeType.TryParse(mem, out MimeType? mime));
       
        Assert.AreEqual("x/y;quoted*=utf-8''text%20loch", mime.ToString(MimeFormats.Url));
    }

    [TestMethod]
    public void ToStringTest3()
    {
        const string input = "x/y; encoded*=utf-8\'\'value%20continuation";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeType.TryParse(mem, out MimeType? mime));
       
        Assert.AreEqual("x/y;encoded*=utf-8\'\'value%20continuation", mime.ToString(MimeFormats.Url));
    }

    [TestMethod]
    public void ToStringTest4()
    {
        const string input = "x/y; normal=value";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeType.TryParse(mem, out MimeType? mime));
        Assert.AreEqual(input, mime.ToString());
    }

    [TestMethod]
    public void ToStringTest5()
    {
        const string input = "x/y; quoted=\"text loch\"";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeType.TryParse(mem, out MimeType? mime));

        Assert.AreEqual(input, mime.ToString());
    }

    [TestMethod]
    public void ToStringTest6()
    {
        string input = "x/y; encoded*=utf-8\'\'" + Uri.EscapeDataString("äöü");
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeType.TryParse(mem, out MimeType? mime));

        Assert.AreEqual(input, mime.ToString());
    }



    [TestMethod]
    public void CharsetTest1()
    {
        string input = "key*=utf-8'de'" + Uri.EscapeDataString("äääääääääh");
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(true, ref mem, out MimeTypeParameterInfo parameter, out _));
        Assert.AreEqual("utf-8", parameter.CharSet.ToString());
        Assert.AreEqual("de", parameter.Language.ToString());

    }




}
