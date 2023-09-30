using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using FolkerKinzel.Strings.Polyfills;
using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;
using System.Text;
using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass()]
public class MimeTypeInfoTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeInfoTest3() => _ = MimeType.Create("", "subtype");

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeInfoTest4() => _ = MimeType.Create("type", "");

    [DataTestMethod]
    [DataRow("?")]
    [DataRow("*")]
    [DataRow("%")]
    [DataRow(" ")]
    [DataRow("ö")]
    [DataRow("\0")]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeInfoTest5(string type) => _ = MimeType.Create(type, "");


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeInfoTest6() => _ = MimeType.Create(new string('a', short.MaxValue + 1), "subtype");


    [TestMethod]
    public void MimeTypeInfoTest7()
    {
        MimeType mime = MimeType.Create("application", "was").AppendParameter("para", "@");
        string s = mime.ToString();
        StringAssert.Contains(s, "\"@\"");
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ParseTest1()
        => _ = MimeTypeInfo.Parse((string?)null!);

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ParseTest2() => _ = MimeTypeInfo.Parse(" \t \r\n");

    [TestMethod]
    public void ParseTest3()
    {
        string mimeString = """
            application/x-stuff;
            key1*0=123;
            key1*1=456;
            key2*0=abc;
            key2*1=def
            """;
        var mime = MimeTypeInfo.Parse(mimeString.AsMemory());
        MimeTypeParameterInfo[] arr = mime.Parameters().ToArray();
        Assert.AreEqual(2, arr.Length);
        Assert.IsNotNull(arr.FirstOrDefault(x => x.Key.Equals("key2", StringComparison.Ordinal) && x.Value.Equals("abcdef", StringComparison.Ordinal)));
        Assert.IsFalse(mime.IsText);
        Assert.IsFalse(mime.IsEmpty);
        Assert.IsFalse(mime.IsOctetStream);
        Assert.IsFalse(mime.IsTextPlain);
    }


    [TestMethod]
    public void ParseTest5()
    {
        const string media = "image";
        const string subType = "jpeg";

        string s = $"  {media}/{subType} (Comment2)";
        var mime = MimeTypeInfo.Parse(s);
        Assert.AreEqual(media, mime.MediaType.ToString());
        Assert.AreEqual(subType, subType.ToString());
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ParseTest6() => _ = MimeTypeInfo.Parse("ääääääää/jpeg");


    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ParseTest7() => _ = MimeTypeInfo.Parse("image/ääääääää");

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ParseTest8()
    {
        string veryLong = new('a', short.MaxValue + 1);
        _ = MimeTypeInfo.Parse($"image/{veryLong}");
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ParseTest9()
    {
        string veryLong = new('a', short.MaxValue + 1);
        _ = MimeTypeInfo.Parse($"{veryLong}/jpeg");
    }

    [TestMethod]
    public void ParseTest10() => Assert.AreEqual("image/png", MimeTypeInfo.Parse("image / png").ToString(), false);

    [TestMethod]
    public void ParseTest11() => Assert.AreEqual("image/png", MimeTypeInfo.Parse("image/png (Comment); key=value").ToString(MimeFormats.IgnoreParameters), false);


    [DataTestMethod]
    [DataRow("text/plain; charset=iso-8859-1", true, 1)]
    [DataRow("text / plain; charset=iso-8859-1;;", true, 1)]
    [DataRow("text / plain; charset=iso-8859-1;second=;", true, 2)]
    [DataRow("text / plain; charset=iso-8859-1;second=\"Second ; Value\"", true, 2)]
    public void TryParseTest1(string input, bool expected, int parametersCount)
    {
        Assert.AreEqual(expected, MimeTypeInfo.TryParse(input, out MimeTypeInfo mediaType));

        //int size = Marshal.SizeOf(ReadOnlyMemory<char>.Empty);
        //size = Marshal.SizeOf(mediaType);

        MimeTypeParameterInfo[]? arr = mediaType.Parameters().ToArray();

        Assert.AreEqual(parametersCount, arr.Length);
        Assert.IsTrue(mediaType.IsText);
        Assert.IsTrue(mediaType.IsTextPlain);
        Assert.IsFalse(mediaType.IsEmpty);
        Assert.IsFalse(mediaType.IsOctetStream);
    }


    [TestMethod]
    public void TryParseTest2()
    {
        string mimeString = """
            application/x-stuff;k=val1;
            key2*0=abc;
            key2*1=def
            """;
        Assert.IsTrue(MimeType.TryParse(mimeString.AsMemory(), out MimeType? mime));
        MimeTypeParameter[] arr = mime.Parameters.ToArray();
        Assert.IsNotNull(arr.FirstOrDefault(x => StringComparer.Ordinal.Equals(x.Key, "key2") && StringComparer.Ordinal.Equals(x.Value, "abcdef")));

        string s = mime.ToString(MimeFormats.LineWrapping, 10);
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
    }

    [TestMethod]
    public void TryParseTest2b()
    {
        string mimeString = """
            application/x-stuff;
            key2*1=abc;
            key2*2=def
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString, out MimeTypeInfo mime));
        Assert.IsFalse(mime.Parameters().Any());
    }

    [TestMethod]
    public void TryParseTest2c()
    {
        string mimeString = """
            application/x-stuff;
            key2*0=abc;
            key2*2=def
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString, out MimeTypeInfo mime));
        Assert.IsFalse(mime.Parameters().Any());
    }

    [TestMethod]
    public void TryParseTest2d()
    {
        string mimeString = """
            application/x-stuff;
            key2*0=abc;
            key2*0=def
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString, out MimeTypeInfo mime));
        Assert.IsFalse(mime.Parameters().Any());
    }

    [TestMethod]
    public void TryParseTest2e()
    {
        string mimeString = """
            application/x-stuff;
            key2*x=abc;
            key2*1=def
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString, out MimeTypeInfo mime));
        Assert.IsFalse(mime.Parameters().Any());
    }


    [TestMethod]
    public void TryParseTest3()
    {
        string mimeString = """
            application/x-stuff;k=val1;
            key2*0=abc;
            key2*1=def
            """;
        Assert.IsTrue(MimeType.TryParse(mimeString.AsMemory(), out MimeType? mime));
        MimeTypeParameter[] arr = mime.Parameters.ToArray();
        Assert.IsNotNull(arr.FirstOrDefault(x => StringComparer.Ordinal.Equals(x.Key, "key2") && StringComparer.Ordinal.Equals(x.Value, "abcdef")));

        string s = mime.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
    }


    [TestMethod]
    public void TryParseTest4()
    {
        string urlEncoded = Uri.EscapeDataString("äöü");
        string mimeString = $"""
            application/x-stuff;
            key*0*=utf-8'en'{urlEncoded} ;
            key*1="This is %EF 7e\\ / \" @+? ; quoted.\";
            key*2=1+2 %A5
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString.AsMemory(), out MimeTypeInfo mime));
        MimeTypeParameterInfo[] paras = mime.Parameters().ToArray();
        Assert.AreEqual(1, paras.Length);

        MimeTypeParameterInfo par = paras[0];
        Assert.AreEqual("key", par.Key.ToString());
        string value = par.Value.ToString();
        StringAssert.Contains(value, @"This is %EF 7e\ / "" @+? ; quoted.\1+2 %A5");
        Assert.AreEqual("en", par.Language.ToString());
        Assert.AreEqual("utf-8", par.CharSet.ToString());
    }

    [TestMethod]
    public void TryParseTest4e()
    {
        string urlEncoded = Uri.EscapeDataString("äöü");
        string mimeString = $"""
            application/x-stuff;
            key*0*=utf-8'en'{urlEncoded} ;
            key*1="1+2 A5";
            key*2="This is %EF 7e\\ / \" @+? ; quoted.\";
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString.AsMemory(), out MimeTypeInfo mime));
        MimeTypeParameterInfo[] paras = mime.Parameters().ToArray();
        Assert.AreEqual(1, paras.Length);

        MimeTypeParameterInfo par = paras[0];
        Assert.AreEqual("key", par.Key.ToString());
        string value = par.Value.ToString();
        StringAssert.Contains(value, @"1+2 A5This is %EF 7e\ / "" @+? ; quoted.\");
        Assert.AreEqual("en", par.Language.ToString());
        Assert.AreEqual("utf-8", par.CharSet.ToString());
    }

    [TestMethod]
    public void TryParseTest4d()
    {
        const string charSet = "iso-8859-1";
        string urlEncoded = UrlEncodingHelper.UrlEncodeWithCharset(charSet, "äöü");
        string mimeString = $"""
            application/x-stuff;
            key*0*={charSet}'en'{urlEncoded} ;
            key*1="This is %EF 7e\\ / \" @+? ; quoted.\";
            key*2=1+2 %A5
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString.AsMemory(), out MimeTypeInfo mime));
        MimeTypeParameterInfo[] paras = mime.Parameters().ToArray();
        Assert.AreEqual(1, paras.Length);

        MimeTypeParameterInfo par = paras[0];
        Assert.AreEqual("key", par.Key.ToString());
        string value = par.Value.ToString();
        StringAssert.Contains(value, @"This is %EF 7e\ / "" @+? ; quoted.\1+2 %A5");
        Assert.AreEqual("en", par.Language.ToString());
        Assert.AreEqual(charSet, par.CharSet.ToString());
    }


    [TestMethod]
    public void TryParseTest4b()
    {
        string mimeString = $"""
            application/x-stuff;
            key*0=abc;
            key*1=xy%EFz
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString.AsMemory(), out MimeTypeInfo mime));
        MimeTypeParameterInfo[] paras = mime.Parameters().ToArray();
        Assert.AreEqual(1, paras.Length);

        MimeTypeParameterInfo par = paras[0];
        Assert.AreEqual("key", par.Key.ToString());
        StringAssert.Contains(par.Value.ToString(), "xy%EFz");
        Assert.AreEqual("", par.Language.ToString());
        Assert.AreEqual("", par.CharSet.ToString());
    }

    [TestMethod]
    public void TryParseTest4c()
    {
        string mimeString = $"""
            application/x-stuff;
            key*0="abc def";
            key*1="xy %EF z"
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString.AsMemory(), out MimeTypeInfo mime));
        MimeTypeParameterInfo[] paras = mime.Parameters().ToArray();
        Assert.AreEqual(1, paras.Length);

        MimeTypeParameterInfo par = paras[0];
        Assert.AreEqual("key", par.Key.ToString());
        StringAssert.Contains(par.Value.ToString(), "xy %EF z");
        Assert.AreEqual("", par.Language.ToString());
        Assert.AreEqual("", par.CharSet.ToString());
    }


    [TestMethod]
    public void TryParseTest5()
    {
        const string ab = @"a\\b";
        string mimeString = $"""
            application/x-stuff;
            key*0="a\\\";
            key*1="\b"
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString.AsMemory(), out MimeTypeInfo mime));

        MimeTypeParameterInfo[] arr = mime.Parameters().ToArray();
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(ab, arr[0].Value.ToString(), false);
    }

    [TestMethod]
    public void TryParseTest6()
    {
        const string ab = @"a\\b";
        string mimeString = $"""
            application/x-stuff;
            key*0="a\\";
            key*1="\\b"
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString.AsMemory(), out MimeTypeInfo mime));

        MimeTypeParameterInfo[] arr = mime.Parameters().ToArray();
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(ab, arr[0].Value.ToString(), false);
    }

    [TestMethod]
    public void TryParseTest7()
    {
        const string ab = @"a\\b";
        string mimeString = $"""
            application/x-stuff;
            key*0="a\"        ;
            key*1="\\\b"
            """;
        Assert.IsTrue(MimeTypeInfo.TryParse(mimeString.AsMemory(), out MimeTypeInfo mime));

        MimeTypeParameterInfo[] arr = mime.Parameters().ToArray();
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(ab, arr[0].Value.ToString(), false);
    }

    [TestMethod]
    public void TryParseTest8()
    {
        const string input = "ibm pm metafile";

        Assert.IsFalse(MimeTypeInfo.TryParse(input, out _));
    }

    [TestMethod]
    public void TryParseTest9()
    {
        const string input = "media/sub; äöü*0=abc; äöü*1=def; key=value";
        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo info));
        Assert.IsFalse(info.Parameters().Any());
    }

    [TestMethod]
    public void TryParseTest10()
    {
        const string input = "media/sub; äöü*0=abc; äöü*1=def; key*0=value0; key*1=value1;";
        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo info));
        Assert.IsFalse(info.Parameters().Any());
    }

    [TestMethod]
    public void TryParseTest11()
    {
        const string input = "media/sub; key=value; äöü*0=abc; äöü*1=def";
        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo info));
        Assert.AreEqual(1, info.Parameters().Count());
    }

    [TestMethod]
    public void TryParseTest12()
    {
        const string input = "media/sub; key*0=value0; key*1=value1; äöü*0=abc; äöü*1=def";
        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo info));
        Assert.AreEqual(1, info.Parameters().Count());
    }

    [TestMethod]
    public void TryParseTest13()
    {
        const string input = "media/sub; par*0*=nixda'en'a%42c; par*1=def; key=value";
        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo info));
        Assert.IsFalse(info.Parameters().Any());
    }

    [TestMethod]
    public void TryParseTest14()
    {
        const string input = "media/sub; par*0*=nixda'en'a%42c; par*1=def; key*0=value0; key*1=value1;";
        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo info));
        Assert.IsFalse(info.Parameters().Any());
    }

    [DataTestMethod]
    [DataRow("äü")]
    [DataRow("{!")]
    public void TryParseTest15(string input)
    {
        string media = $"media/sub; par*=utf-8'{input}'abc;";
        Assert.IsTrue(MimeTypeInfo.TryParse(media, out MimeTypeInfo info));
        Assert.IsFalse(info.Parameters().Any());
    }

    [TestMethod]
    public void TryParseTest16()
    {
        string media = $"media/sub; par*=utf-8'{new string('a', 256)}'abc;";
        Assert.IsTrue(MimeTypeInfo.TryParse(media, out MimeTypeInfo info));
        Assert.IsFalse(info.Parameters().Any());
    }

    [TestMethod()]
    public void ToStringTest1()
    {
        string result = new MimeTypeInfo().ToString();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }


    [TestMethod()]
    public void ToStringTest2()
    {
        const string input = "text/plain";
        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo media));
        string result = media.ToString();
        Assert.AreEqual(input, result);
    }


    [TestMethod]
    public void ToStringTest3()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeTypeInfo inetMedia));

        Assert.AreEqual("text/plain; charset=iso-8859-1", inetMedia.ToString(), true);
    }

    [TestMethod]
    public void ToStringTest4()
    {
        string input = "application/x-stuff; p1=normal; p2=\"text loch\"; p3*=utf-8\'\'" + Uri.EscapeDataString("äöü");
        var mime = MimeType.Parse(input);
        Assert.AreEqual("application/x-stuff;p1=normal;p2*=utf-8\'\'text%20loch;p3*=utf-8\'\'" + Uri.EscapeDataString("äöü"), mime.ToString(MimeFormats.Url));
    }

    [TestMethod]
    public void ToStringTest5()
    {
        string value = "";

        for (int i = 0; i < 50; i++)
        {
            value += "abc";
        }

        MimeType mime = MimeType.Create("application", "x-stuff")
                                  .AppendParameter("key", value)
                                  .AppendParameter("other", "bla");

        string s = mime.ToString(MimeFormats.LineWrapping);
        Assert.IsFalse(s.Contains('\"'));
        Assert.IsFalse(s.Contains('\''));

        mime = MimeType.Parse(s);
        Assert.AreEqual(2, mime.Parameters.Count());
    }

    [TestMethod]
    public void ToStringTest6()
    {
        const string charSet = "UTF-8";
        string result = MimeTypeInfo.Parse($"application/x-stuff; charset={charSet}").ToString();
        Assert.IsFalse(result.Contains(charSet, StringComparison.Ordinal));
        Assert.IsTrue(result.Contains(charSet, StringComparison.OrdinalIgnoreCase));
    }


    [TestMethod]
    public void ToStringTest7()
    {
        const string accessType = "\"ABC\\\\42\"";
        string result = MimeTypeInfo.Parse($"application/x-stuff; access-type={accessType}").ToString();
        Assert.IsFalse(result.Contains(accessType, StringComparison.Ordinal));
        Assert.IsTrue(result.Contains(accessType, StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void ToStringTest8()
    {
        const string otherValue = "\"ABC\\\\42\"";
        string result = MimeTypeInfo.Parse($"application/x-stuff; other={otherValue}").ToString();
        Assert.IsTrue(result.Contains(otherValue, StringComparison.Ordinal));
    }

    [TestMethod]
    public void ToStringTest9()
    {
        MimeType mime = MimeType.Create("application", "x-stuff")
            .AppendParameter("keyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "valueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");

        string s = mime.AsInfo().ToString(options: MimeFormats.LineWrapping, lineLength: 10);
        Assert.AreNotEqual(0, s.Length);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AppendToTest1()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeTypeInfo inetMedia));
        inetMedia.AppendTo(null!);
    }

    [TestMethod]
    public void AppendToTest2()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeTypeInfo inetMedia));
        var builder = new StringBuilder();
        inetMedia.AppendTo(builder);
        Assert.AreEqual("text/plain; charset=iso-8859-1", builder.ToString());
    }

    [TestMethod]
    public void AppendToTest3() => Assert.AreEqual(0, new MimeTypeInfo().AppendTo(new StringBuilder()).Length);

    [TestMethod]
    public void AppendToTest4()
    {
        const string input = "text/plain";
        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo media));
        Assert.AreEqual(input, media.AppendTo(new StringBuilder()).ToString(), false);
    }

        [TestMethod]
    public void CloneTest1()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=US-ASCII", out MimeTypeInfo media1));
        ICloneable o1 = media1;

        object o2 = o1.Clone();

        Assert.IsTrue(o1.Equals(o2));
    }

    [TestMethod]
    public void CloneTest2()
    {
        var o = (MimeTypeInfo)MimeTypeInfo.Empty.Clone();
        Assert.IsTrue(o.IsEmpty);
    }



    [TestMethod]
    public void EqualsTest1()
    {
        string media1 = "text/plain; charset=iso-8859-1";
        string media2 = "TEXT/PLAIN; CHARSET=ISO-8859-1";

        var mediaType1 = MimeTypeInfo.Parse(media1);
        var mediaType2 = MimeTypeInfo.Parse(media2);

        Assert.IsTrue(mediaType1 == mediaType2);
        Assert.IsFalse(mediaType1 != mediaType2);

        Assert.AreEqual(mediaType1.GetHashCode(), mediaType2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest2()
    {
        string media1 = "text/plain; charset=iso-8859-1;second=value";
        string media2 = "TEXT/PLAIN; CHARSET=ISO-8859-1;SECOND=VALUE";

        var mediaType1 = MimeTypeInfo.Parse(media1);
        var mediaType2 = MimeTypeInfo.Parse(media2);

        Assert.IsTrue(mediaType1 != mediaType2);
        Assert.IsFalse(mediaType1 == mediaType2);

        Assert.AreNotEqual(mediaType1.GetHashCode(), mediaType2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest3()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; CharSet=Us-Ascii", out MimeTypeInfo media1));
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain", out MimeTypeInfo media2));

        Assert.IsTrue(media1 == media2);
        Assert.IsFalse(media1 != media2);

        Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest4()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=iso-8859-1", out MimeTypeInfo media1));
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain", out MimeTypeInfo media2));

        Assert.IsTrue(media1 != media2);
        Assert.IsFalse(media1 == media2);

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest5()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=iso-8859-1", out MimeTypeInfo media1));
        Assert.IsTrue(MimeTypeInfo.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeTypeInfo media2));

        Assert.IsTrue(media1 == media2);
        Assert.IsFalse(media1 != media2);

        Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest6()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=iso-8859-1;other=value", out MimeTypeInfo media1));
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain;charset=iso-8859-1;OTHER=VALUE", out MimeTypeInfo media2));

        Assert.IsTrue(media1 != media2);
        Assert.IsFalse(media1 == media2);

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest7()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=iso-8859-1", out MimeTypeInfo media1));
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain", out MimeTypeInfo media2));

        Assert.IsFalse(media1.Equals(media2));
        Assert.IsFalse(media1.Equals(in media2));
        Assert.IsFalse(media1.Equals(in media2, false));
        Assert.IsTrue(media1.Equals(in media2, true));

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        Assert.AreEqual(media1.GetHashCode(true), media2.GetHashCode(true));
    }

    [TestMethod]
    public void EqualsTest8()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=US-ASCII", out MimeTypeInfo media1));
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain", out MimeTypeInfo media2));

        Assert.IsTrue(media1.Equals(media2));
        Assert.IsTrue(media1.Equals(in media2));
        Assert.IsTrue(media1.Equals(in media2, false));
        Assert.IsTrue(media1.Equals(in media2, true));

        Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
        Assert.AreEqual(media1.GetHashCode(true), media2.GetHashCode(true));
    }


    [TestMethod]
    public void EqualsTest9()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=US-ASCII", out MimeTypeInfo media1));
        object o1 = media1;
        Assert.IsFalse(o1.Equals(42));
    }

    [TestMethod]
    public void EqualsTest10()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("application/octet-stream; charset=US-ASCII", out MimeTypeInfo media1));
        Assert.IsTrue(MimeTypeInfo.TryParse("application/octet-stream", out MimeTypeInfo media2));

        Assert.IsFalse(media1.Equals(media2));
        Assert.IsFalse(media1.Equals(in media2));
        Assert.IsFalse(media1.Equals(in media2, false));
        Assert.IsTrue(media1.Equals(in media2, true));

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        Assert.AreEqual(media1.GetHashCode(true), media2.GetHashCode(true));
    }

    [TestMethod]
    public void EqualsTest11()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("text/plain; charset=US-ASCII", out MimeTypeInfo media1));
        Assert.IsTrue(MimeTypeInfo.TryParse("application/octet-stream", out MimeTypeInfo media2));

        //Assert.IsTrue(media1.IsTextPlain);
        //Assert.IsFalse(media2.IsTextPlain);

        Assert.IsFalse(media1.Equals(media2));
        Assert.IsFalse(media1.Equals(in media2));
        Assert.IsFalse(media1.Equals(in media2, false));
        Assert.IsFalse(media1.Equals(in media2, true));

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
        MimeTypeInfo.Parse(media2) != MimeTypeInfo.Parse(media3) &&
        MimeTypeInfo.Parse(media2) != MimeTypeInfo.Parse(media4) &&
        MimeTypeInfo.Parse(media2).Equals(MimeTypeInfo.Parse(media4), ignoreParameters: true) &&
        MimeTypeInfo.Parse(media4) == MimeTypeInfo.Parse(media5) &&
        MimeTypeInfo.Parse(media4) != MimeTypeInfo.Parse(media6) &&
        MimeTypeInfo.Parse(media6) == MimeTypeInfo.Parse(media7) &&
        MimeTypeInfo.Parse(media6) != MimeTypeInfo.Parse(media8));

    }

    [TestMethod]
    public void RemoveUrlEncodingTest1()
    {
        Assert.IsTrue(MimeTypeInfo.TryParse("application/octet-stream; para*=utf-8'de'Hallo%20Folker", out MimeTypeInfo media1));

        MimeTypeParameterInfo para = media1.Parameters().First();
        Assert.AreEqual(para.Value.ToString(), "Hallo Folker");
        Assert.AreEqual(para.Language.ToString(), "de");
        Assert.AreEqual(para.Key.ToString(), "para");
    }

    [TestMethod]
    public void ParseParametersTest1()
    {
        string input = "application/x-stuff;" + Environment.NewLine +
                        "title*0*=us-ascii'en'This%20is%20even%20more%20;" + Environment.NewLine +
                        "title*1*=%2A%2A%2Afun%2A%2A%2A%20;" + Environment.NewLine +
                        "title*2=\"isn't it!\"";

        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo mimeType));
        Assert.AreEqual(1, mimeType.Parameters().Count());
        MimeTypeParameterInfo param = mimeType.Parameters().First();

        Assert.AreEqual("title", param.Key.ToString());
        Assert.AreEqual("en", param.Language.ToString());
        Assert.IsTrue(param.Value.EndsWith("isn't it!".AsSpan()));

    }

    [TestMethod]
    public void ParseParametersTest2()
    {
        const string input = "application/x-stuff; param=\"directory\\\\file.text\"";

        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo mimeType));
        Assert.AreEqual(1, mimeType.Parameters().Count());
        MimeTypeParameterInfo param = mimeType.Parameters().First();

        Assert.AreEqual("param", param.Key.ToString());
        Assert.AreEqual(@"directory\file.text", param.Value.ToString());

        Assert.AreEqual(input, MimeType.Create(in mimeType).ToString());
    }

    [DataTestMethod]
    [DataRow("text/cache-manifest", ".appcache")]
    [DataRow("text/n3", ".n3")]
    [DataRow("text/tab-separated-values", ".tsv")]
    [DataRow("text/x-asm", ".s")]
    [DataRow("video/vnd.dece.hd", ".uvh")]
    [DataRow("image/ief", ".ief")]
    [DataRow("image/vnd.fujixerox.edmics-rlc", ".rlc")]
    [DataRow("application/cu-seeme", ".cu")]
    [DataRow("audio/silk", ".sil")]
    [DataRow("chemical/x-cdx", ".cdx")]
    [DataRow("model/3mf", ".3mf")]
    [DataRow("nixda/nischgefunden", ".bin")]
    public void GetFileTypeExtensionTest1(string mime, string expected)
        => Assert.AreEqual(expected, MimeTypeInfo.Parse(mime).GetFileTypeExtension());

    [TestMethod]
    public void GetFileTypeExtensionTest2() => Assert.AreEqual(".bin", new MimeTypeInfo().GetFileTypeExtension());

    [DataTestMethod]
    [DataRow("text/cache-manifest", ".appcache")]
    [DataRow("text/n3", ".n3")]
    [DataRow("text/tab-separated-values", ".tsv")]
    [DataRow("text/x-asm", ".s")]
    [DataRow("video/vnd.dece.hd", ".uvh")]
    [DataRow("image/ief", ".ief")]
    [DataRow("image/vnd.fujixerox.edmics-rlc", ".rlc")]
    [DataRow("application/cu-seeme", ".cu")]
    [DataRow("audio/silk", ".sil")]
    [DataRow("chemical/x-cdx", ".cdx")]
    [DataRow("workbook/formulaone", ".vts")]
    public void FromFileTypeExtensionTest1(string expected, string fileTypeExtension)
        => Assert.AreEqual(expected, MimeString.FromFileName(fileTypeExtension));

    [TestMethod]
    public void FromFileTypeExtensionTest2()
        => Assert.AreEqual("application/octet-stream", MimeString.FromFileName(""));

    [TestMethod]
    public void FromFileTypeExtensionTest3()
        => Assert.AreEqual("application/octet-stream", MimeString.FromFileName("".AsSpan()));

    //[TestMethod]
    //public void FromFileTypeExtensionTest4()
    //{
    //    var mime = MimeTypeInfo.FromFileName((string)null!);
    //    Assert.AreEqual("application/octet-stream", mime.ToString());
    //}

    [TestMethod]
    public void BuildAndParseTest1()
    {
        const string mediaType = "application";
        const string subType = "x-stuff";

        var dic = new ParameterModelDictionary()
        {
            new MimeTypeParameter("first-parameter",
            "This is a very long parameter, which will be wrapped according to RFC 2184." +
            Environment.NewLine +
            "It contains also a few Non-ASCII-Characters: \u00E4\u00D6\u00DF.", "en"),
            new MimeTypeParameter("second-parameter", "Parameter with  \\, = and \".", null)
        };

        var mimeType1 = new MimeTypeInfo(mediaType, subType, dic);
        string s = MimeType.Create(in mimeType1).ToString(MimeFormats.LineWrapping);

        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
        Assert.AreNotEqual(1, s.GetLinesCount());

        var mimeType2 = MimeTypeInfo.Parse(s);

        Assert.AreEqual(mediaType, mimeType2.MediaType.ToString(), false);
        Assert.AreEqual(subType, mimeType2.SubType.ToString(), false);

        Assert.AreEqual(2, mimeType2.Parameters().Count());
    }

    [TestMethod]
    public void BuildAndParseTest2()
    {
        Debug.WriteLine('\uFFFD');
        const string mediaType = "application";
        const string subType = "x-stuff";
        const string paraKey = "key";
        string paraValue = "a@b@c " + new string('a', 100);

        string mimeString = $"{mediaType}/{subType};{paraKey}=\"{paraValue}\"";


        var mimeType1 = MimeTypeInfo.Parse(mimeString);
        string s = MimeType.Create(in mimeType1).ToString(MimeFormats.LineWrapping, 10);

        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
        Assert.AreNotEqual(1, s.GetLinesCount());

        var mimeType2 = MimeTypeInfo.Parse(s);

        Assert.AreEqual(mediaType, mimeType2.MediaType.ToString(), false);
        Assert.AreEqual(subType, mimeType2.SubType.ToString(), false);

        Assert.AreEqual(1, mimeType2.Parameters().Count());
    }

    [TestMethod]
    public void SubTypeTest1() => Assert.AreEqual(0, new MimeTypeInfo().SubType.Length);

}