﻿using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;
using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Tests;

[TestClass]
public class ParameterSerializerTests
{
    [TestMethod]
    public void AppendTest1()
    {
        var model = new ParameterModel("key", null);

        var sb = new StringBuilder();
        ParameterSerializer.Append(sb, in model);
        StringAssert.Contains(sb.ToString(), "key=\"\"");
    }

    [TestMethod]
    public void AppendTest2()
    {
        var model = new ParameterModel("key", null, "en");

        var sb = new StringBuilder();
        ParameterSerializer.Append(sb, in model);
        StringAssert.Contains(sb.ToString(), "key*=utf-8'en'");
    }

    [TestMethod]
    public void AppendTest3()
    {
        var sb = new StringBuilder();
        const string nonAscii = "ä";
        const string ascii = "para";

        ParameterSerializer.Append(sb, new ParameterModel(ascii, nonAscii));
        Assert.AreNotEqual(0, sb.Length);
        string s = sb.ToString();
        Assert.IsTrue(s.Contains(ascii));
        Assert.IsFalse(s.Contains(nonAscii));
    }

    [TestMethod]
    public void AppendTest4()
    {
        ReadOnlyMemory<char> mem = "key=".AsMemory();
        _ = MimeTypeParameter.TryParse(true, ref mem, out MimeTypeParameter para, out _);

        var sb = new StringBuilder();
        sb.Append(in para, false);
        StringAssert.Contains(sb.ToString(), "key=\"\"");
    }

    [TestMethod]
    public void AppendTest5()
    {
        ReadOnlyMemory<char> mem = "key*='en'".AsMemory();
        _ = MimeTypeParameter.TryParse(true, ref mem, out MimeTypeParameter para, out _);

        var sb = new StringBuilder();
        sb.Append(in para, false);
        StringAssert.Contains(sb.ToString(), "key*=utf-8'en'");
    }

    [TestMethod]
    public void AppendTest6()
    {
        ReadOnlyMemory<char> mem = ("key*=''" + Uri.EscapeDataString("äöü")).AsMemory();
        _ = MimeTypeParameter.TryParse(true, ref mem, out MimeTypeParameter para, out _);

        var sb = new StringBuilder();
        sb.Append(in para, false);
        StringAssert.Contains(sb.ToString(), "key*=");
    }

    [TestMethod]
    public void AppendTest7()
    {
        const string bla = "charset=\"BLA\\\"BLA\"";
        ReadOnlyMemory<char> mem = bla.AsMemory();
        _ = MimeTypeParameter.TryParse(true, ref mem, out MimeTypeParameter para, out _);

        var sb = new StringBuilder();
        sb.Append(in para, false);
        StringAssert.Contains(sb.ToString(), bla.ToLowerInvariant());
    }
}

