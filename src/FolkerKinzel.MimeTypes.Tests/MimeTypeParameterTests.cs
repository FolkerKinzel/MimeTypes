﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        Assert.IsTrue(MimeTypeInfo.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeTypeInfo inetMedia));
        ICloneable cloneable = inetMedia.Parameters().First();
        object o = cloneable.Clone();
        Assert.AreEqual(cloneable, o);
        Assert.AreEqual("charset=iso-8859-1", o.ToString());
    }

    [TestMethod]
    public void EqualsTest1()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=US-ASCII", out MimeTypeInfo media1));
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
    [ExpectedException(typeof(ArgumentNullException))]
    public void AppendToTest1() => MimeTypeParameterInfo.Empty.AppendTo(null!);


    [TestMethod]
    public void AppendToTest2()
    {
        var sb = new StringBuilder();
        MimeTypeParameterInfo.Empty.AppendTo(sb);
        Assert.AreEqual(0, sb.Length);
    }

    [TestMethod]
    public void AppendToTest3()
    {
        const string input = "normal=value";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(false, ref mem, out MimeTypeParameterInfo para, out _));
        var sb = new StringBuilder();
        _ = para.AppendTo(sb, urlFormat: true);
        Assert.AreEqual(input, sb.ToString());
    }

    [TestMethod]
    public void AppendToTest4()
    {
        const string input = "quoted=\"text loch\"";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(false, ref mem, out MimeTypeParameterInfo para, out _));
        var sb = new StringBuilder();
        _ = para.AppendTo(sb, urlFormat: true);
        Assert.AreEqual("quoted*=utf-8''text%20loch", sb.ToString());
    }

    [TestMethod]
    public void AppendToTest5()
    {
        const string input = "encoded*=utf-8''value%20continuation";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(false, ref mem, out MimeTypeParameterInfo para, out _));
        var sb = new StringBuilder();
        _ = para.AppendTo(sb, urlFormat: true);
        Assert.AreEqual(input, sb.ToString());
    }


    [TestMethod]
    public void ToStringTest1()
    {
        const string input = "normal=value";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(false, ref mem, out MimeTypeParameterInfo para, out _));
        Assert.AreEqual(input, para.ToString(urlFormat: true));
    }

    [TestMethod]
    public void ToStringTest2()
    {
        const string input = "quoted=\"text loch\"";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(false, ref mem, out MimeTypeParameterInfo para, out _));
       
        Assert.AreEqual("quoted*=utf-8''text%20loch", para.ToString(urlFormat: true));
    }

    [TestMethod]
    public void ToStringTest3()
    {
        const string input = "encoded*=utf-8\'\'value%20continuation";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(false, ref mem, out MimeTypeParameterInfo para, out _));
       
        Assert.AreEqual(input, para.ToString(urlFormat: true));
    }

    [TestMethod]
    public void ToStringTest4()
    {
        const string input = "normal=value";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(false, ref mem, out MimeTypeParameterInfo para, out _));
        Assert.AreEqual(input, para.ToString());
    }

    [TestMethod]
    public void ToStringTest5()
    {
        const string input = "quoted=\"text loch\"";
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(false, ref mem, out MimeTypeParameterInfo para, out _));

        Assert.AreEqual(input, para.ToString());
    }

    [TestMethod]
    public void ToStringTest6()
    {
        string input = "encoded*=utf-8\'\'" + Uri.EscapeDataString("äöü");
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsTrue(MimeTypeParameterInfo.TryParse(false, ref mem, out MimeTypeParameterInfo para, out _));

        Assert.AreEqual(input, para.ToString());
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
