using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.MimeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass()]
public class MimeTypeTests
{
    [DataTestMethod()]
    [DataRow(null, "sub")]
    [DataRow("media", null)]
    [DataRow(null, null)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreateTest1(string? nedia, string? sub) => _ = MimeType.Create(nedia!, sub!);

    [TestMethod]
    public void CreateTest2()
    {
        MimeTypeInfo mime1 = MimeType.Create("a", "b")
                                   .AppendParameter("c", "d", "en")
                                   .AppendParameter("e", "f")
                                   .AsInfo();

        MimeTypeInfo mime2 = MimeType.Create(in mime1).AsInfo();

        Assert.AreEqual(mime1.ToString(), mime2.ToString());
    }

    [TestMethod]
    public void ParseTest1()
    {
        ReadOnlyMemory<char> mem = "image/jpeg".AsMemory();
        Assert.IsNotNull(MimeType.Parse(in mem));
    }

    [TestMethod]
    public void TryParseTest1()
    {
        Assert.IsFalse(MimeType.TryParse("blabla", out _));
    }

    [TestMethod]
    public void TryParseTest1b()
    {
        Assert.IsFalse(MimeType.TryParse("blabla".AsMemory(), out _));
    }

    [TestMethod]
    public void TryParseTest1c()
    {
        const string mimeString = "image/png";
        const string extension = ".png";


        ReadOnlyMemory<char> mem = mimeString.AsMemory();
        Assert.IsTrue(MimeType.TryParse(in mem, out MimeType? mime));
        Assert.AreEqual(extension, mime.GetFileTypeExtension());

        Assert.AreEqual(MimeType.FromFileName(extension), mime);
    }

    [TestMethod]
    [ExpectedException (typeof(ArgumentNullException))]
    public void TryParseTest2()
    {
        Assert.IsFalse(MimeType.TryParse(null!, out _));
    }

    [TestMethod()]
    public void ClearParametersTest1()
    {
        MimeType builder = MimeType.Create("text", "xml")
                   .AppendParameter("charset", "utf-8")
                   .AppendParameter("charset", "UTF-8");

        MimeTypeInfo mime = builder.AsInfo();
        Assert.AreEqual(1, mime.Parameters().Count());
        builder.ClearParameters();
        mime = builder.AsInfo();
        Assert.AreEqual(0, mime.Parameters().Count());
    }

    [TestMethod()]
    public void ClearParametersTest2()
    {
        MimeType builder = MimeType.Create("text", "xml").ClearParameters();
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RemoveParametersTest1() => MimeType.Create("x", "y").RemoveParameter(null!);

    [TestMethod]
    public void RemoveParametersTest2() => MimeType.Create("x", "y").RemoveParameter("nichda").RemoveParameter("");

    [TestMethod]
    public void RemoveParametersTest3()
    {
        MimeTypeInfo mime = MimeType.Create("x", "y")
            .AppendParameter("key1", "val")
            .AppendParameter("key2", "val", "en")
            .RemoveParameter("key1")
            .AsInfo();

        MimeTypeParameterInfo[] paras = mime.Parameters().ToArray();
        Assert.AreEqual(1, paras.Length);
        Assert.AreEqual("key2", paras[0].Key.ToString());
    }

    [TestMethod]
    [ExpectedException (typeof(ArgumentNullException))]
    public void AppendToTest1()
    {
        _ = MimeType.Create("x", "y").AppendTo(null!);
    }

    [TestMethod]
    public void AppendToTest2()
    {
        _ = MimeType.Create("x", "y").AppendParameter("a", "b").AppendTo(new StringBuilder(), MimeFormats.LineWrapping, -42);
    }


    [TestMethod]
    public void EqualsTest1()
    {
        string media1 = "text/plain; charset=iso-8859-1";
        string media2 = "TEXT/PLAIN; CHARSET=ISO-8859-1";

        var mediaType1 = MimeType.Parse(media1);
        var mediaType2 = MimeType.Parse(media2);

        Assert.IsTrue(mediaType1 == mediaType2);
        Assert.IsFalse(mediaType1 != mediaType2);

        Assert.AreEqual(mediaType1.GetHashCode(), mediaType2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest2()
    {
        string media1 = "text/plain; charset=iso-8859-1;second=value";
        string media2 = "TEXT/PLAIN; CHARSET=ISO-8859-1;SECOND=VALUE";

        var mediaType1 = MimeType.Parse(media1);
        var mediaType2 = MimeType.Parse(media2);

        Assert.IsTrue(mediaType1 != mediaType2);
        Assert.IsFalse(mediaType1 == mediaType2);

        Assert.AreNotEqual(mediaType1.GetHashCode(), mediaType2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest3()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=us-ascii", out MimeType? media1));
        Assert.IsTrue(MimeType.TryParse("text/plain", out MimeType? media2));

        Assert.IsTrue(media1 == media2);
        Assert.IsFalse(media1 != media2);

        Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest4()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1", out MimeType? media1));
        Assert.IsTrue(MimeType.TryParse("text/plain", out MimeType? media2));

        Assert.IsTrue(media1 != media2);
        Assert.IsFalse(media1 == media2);

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest5()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1", out MimeType? media1));
        Assert.IsTrue(MimeType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeType? media2));

        Assert.IsTrue(media1 == media2);
        Assert.IsFalse(media1 != media2);

        Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest6()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1;other=value", out MimeType? media1));
        Assert.IsTrue(MimeType.TryParse("text/plain;charset=iso-8859-1;OTHER=VALUE", out MimeType? media2));

        Assert.IsTrue(media1 != media2);
        Assert.IsFalse(media1 == media2);

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest7()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1", out MimeType? media1));
        Assert.IsTrue(MimeType.TryParse("text/plain", out MimeType? media2));

        Assert.IsFalse(media1.Equals(media2));
        Assert.IsFalse(media1.Equals(media2));
        Assert.IsFalse(media1.Equals(media2, false));
        Assert.IsTrue(media1.Equals(media2, true));

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        Assert.AreEqual(media1.GetHashCode(true), media2.GetHashCode(true));
    }

    [TestMethod]
    public void EqualsTest8()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=US-ASCII", out MimeType? media1));
        Assert.IsTrue(MimeType.TryParse("text/plain", out MimeType? media2));

        Assert.IsTrue(media1.Equals(media2));
        Assert.IsTrue(media1.Equals(media2));
        Assert.IsTrue(media1.Equals(media2, false));
        Assert.IsTrue(media1.Equals(media2, true));

        Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
        Assert.AreEqual(media1.GetHashCode(true), media2.GetHashCode(true));
    }


    [TestMethod]
    public void EqualsTest9()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=US-ASCII", out MimeType? media1));
        object o1 = media1;
        Assert.IsFalse(o1.Equals(42));
    }

    [TestMethod]
    public void EqualsTest10()
    {
        Assert.IsTrue(MimeType.TryParse("application/octet-stream; charset=US-ASCII", out MimeType? media1));
        Assert.IsTrue(MimeType.TryParse("application/octet-stream", out MimeType? media2));

        Assert.IsFalse(media1.Equals(media2));
        Assert.IsFalse(media1.Equals(media2));
        Assert.IsFalse(media1.Equals(media2, false));
        Assert.IsTrue(media1.Equals(media2, true));

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        Assert.AreEqual(media1.GetHashCode(true), media2.GetHashCode(true));
    }

    [TestMethod]
    public void EqualsTest11()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=US-ASCII", out MimeType? media1));
        Assert.IsTrue(MimeType.TryParse("application/octet-stream", out MimeType? media2));

        Assert.IsFalse(media1.Equals(media2));
        Assert.IsFalse(media1.Equals(media2));
        Assert.IsFalse(media1.Equals(media2, false));
        Assert.IsFalse(media1.Equals(media2, true));

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        Assert.AreNotEqual(media1.GetHashCode(true), media2.GetHashCode(true));
    }

    [TestMethod]
    public void EqualsTest12()
    {
        const string media1 = "text/plain; charset=us-ascii";
        const string media2 = "TEXT/PLAIN";
        const string media3 = "TEXT/HTML";
        const string media4 = "text/plain; charset=iso-8859-1";
        const string media5 = "TEXT/PLAIN; CHARSET=ISO-8859-1";
        const string media6 = "text/plain; charset=iso-8859-1; other-parameter=other_value";
        const string media7 = "text/plain; OTHER-PARAMETER=other_value; charset=ISO-8859-1";
        const string media8 = "text/plain; charset=iso-8859-1; other-parameter=OTHER_VALUE";

        Assert.IsTrue(MimeTypeInfo.Parse(media1) == MimeTypeInfo.Parse(media2) &&
        MimeType.Parse(media2) != MimeType.Parse(media3) &&
        MimeType.Parse(media2) != MimeType.Parse(media4) &&
        MimeType.Parse(media2).Equals(MimeType.Parse(media4), ignoreParameters: true) &&
        MimeType.Parse(media4) == MimeType.Parse(media5) &&
        MimeType.Parse(media4) != MimeType.Parse(media6) &&
        MimeType.Parse(media6) == MimeType.Parse(media7) &&
        MimeType.Parse(media6) != MimeType.Parse(media8));
    }

    [TestMethod]
    public void EqualsTest13()
    {
        Assert.IsFalse(MimeType.Create("x", "y").Equals(42));
    }

    [TestMethod]
    public void EqualsOperatorTest1()
    {
        MimeType? m1 = null;
        MimeType? m2 = null;
        Assert.IsTrue(m1 == m2);
        Assert.IsFalse(m1 != m2);
    }

    [TestMethod]
    public void EqualsOperatorTest2()
    {
        MimeType? m1 = MimeType.Create("x", "y");
        MimeType? m2 = null;
        Assert.IsTrue(m1 != m2);
        Assert.IsTrue(m2 != m1);

        Assert.IsFalse(m1 == m2);
        Assert.IsFalse(m2 == m1);
        m2 = m1;
        Assert.IsTrue(m1 == m2);

    }
}