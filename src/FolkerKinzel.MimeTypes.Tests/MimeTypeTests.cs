using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using FolkerKinzel.Strings.Polyfills;
using FolkerKinzel.MimeTypes.Intls;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass()]
public class MimeTypeTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeTest3() => _ = MimeTypeBuilder.Create("", "subtype");

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeTest4() => _ = MimeTypeBuilder.Create("type", "");

    [DataTestMethod]
    [DataRow("?")]
    [DataRow("*")]
    [DataRow("%")]
    [DataRow(" ")]
    [DataRow("ö")]
    [DataRow("\0")]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeTest5(string type) => _ = MimeTypeBuilder.Create(type, "");


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeTest6() => _ = MimeTypeBuilder.Create(new string('a', short.MaxValue + 1), "subtype");

    [TestMethod]
    public void MimeTypeTest7()
    {
        MimeType mime = MimeTypeBuilder.Create("application", "was").AppendParameter("para", "@").Build();
        string s = mime.ToString();
        StringAssert.Contains(s, "\"@\"");
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ParseTest1()
        => _ = MimeType.Parse((string?)null!);

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ParseTest2() => _ = MimeType.Parse(" \t \r\n");

    [TestMethod]
    public void ParseTest3()
    {
        string mimeString = """
            application/x-stuff;
            key1*1=123;
            key1*2=456;
            key2*1=abc;
            key2*2=def
            """;
        var mime = MimeType.Parse(mimeString.AsMemory());
        MimeTypeParameter[] arr = mime.Parameters().ToArray();
        Assert.AreEqual(2, arr.Length);
        Assert.IsNotNull(arr.FirstOrDefault(x => x.Key.Equals("key2", StringComparison.Ordinal) && x.Value.Equals("abcdef", StringComparison.Ordinal)));
    }


    [TestMethod]
    public void ParseTest5()
    {
        const string media = "image";
        const string subType = "jpeg";

        string s = $"  {media}/{subType} (Comment2)";
        var mime = MimeType.Parse(s);
        Assert.AreEqual(media, mime.MediaType.ToString());
        Assert.AreEqual(subType, subType.ToString());
    }

    [DataTestMethod]
    [DataRow("text/plain; charset=iso-8859-1", true, 1)]
    [DataRow("text / plain; charset=iso-8859-1;;", true, 1)]
    [DataRow("text / plain; charset=iso-8859-1;second=;", true, 2)]
    [DataRow("text / plain; charset=iso-8859-1;second=\"Second ; Value\"", true, 2)]
    public void TryParseTest1(string input, bool expected, int parametersCount)
    {
        Assert.AreEqual(expected, MimeType.TryParse(input, out MimeType mediaType));

        //int size = Marshal.SizeOf(ReadOnlyMemory<char>.Empty);
        //size = Marshal.SizeOf(mediaType);

        MimeTypeParameter[]? arr = mediaType.Parameters().ToArray();

        Assert.AreEqual(parametersCount, arr.Length);
    }


    [TestMethod]
    public void TryParseTest2()
    {
        string mimeString = """
            application/x-stuff;k=val1;
            key2*1=abc;
            key2*2=def
            """;
        Assert.IsTrue(MimeType.TryParse(mimeString.AsMemory(), out MimeType mime));
        MimeTypeParameter[] arr = mime.Parameters().ToArray();
        Assert.IsNotNull(arr.FirstOrDefault(x => x.Key.Equals("key2", StringComparison.Ordinal) && x.Value.Equals("abcdef", StringComparison.Ordinal)));

        string s = mime.ToString();
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
    }


    [TestMethod]
    public void TryParseTest3()
    {
        string mimeString = """
            application/x-stuff;k=val1;
            key2*1=abc;
            key2*2=def
            """;
        Assert.IsTrue(MimeType.TryParse(mimeString.AsMemory(), out MimeType mime));
        MimeTypeParameter[] arr = mime.Parameters().ToArray();
        Assert.IsNotNull(arr.FirstOrDefault(x => x.Key.Equals("key2", StringComparison.Ordinal) && x.Value.Equals("abcdef", StringComparison.Ordinal)));

        string s = mime.ToString(FormattingOptions.IncludeParameters);
        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
    }


    [TestMethod]
    public void TryParseTest4()
    {
        string urlEncoded = Uri.EscapeDataString("äöü");
        string mimeString = $"""
            application/x-stuff;
            key*1*=''{urlEncoded} ;
            key*2="This is %E 7e\\ / \" @ ? ; quoted.\"
            """;
        Assert.IsTrue(MimeType.TryParse(mimeString.AsMemory(), out MimeType mime));
        Assert.AreEqual(1, mime.Parameters().Count());

        var par = mime.Parameters().First();  
    }


    [TestMethod]
    public void TryParseTest5()
    {
        const string ab = @"a\\b";
        string mimeString = $"""
            application/x-stuff;
            key*1="a\\\";
            key*2="\b"
            """;
        Assert.IsTrue(MimeType.TryParse(mimeString.AsMemory(), out MimeType mime));

        MimeTypeParameter[] arr = mime.Parameters().ToArray();
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(ab, arr[0].Value.ToString(), false);
    }

    [TestMethod]
    public void TryParseTest6()
    {
        const string ab = @"a\\b";
        string mimeString = $"""
            application/x-stuff;
            key*1="a\\";
            key*2="\\b"
            """;
        Assert.IsTrue(MimeType.TryParse(mimeString.AsMemory(), out MimeType mime));

        MimeTypeParameter[] arr = mime.Parameters().ToArray();
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(ab, arr[0].Value.ToString(), false);
    }

    [TestMethod]
    public void TryParseTest7()
    {
        const string ab = @"a\\b";
        string mimeString = $"""
            application/x-stuff;
            key*1="a\"        ;
            key*2="\\\b"
            """;
        Assert.IsTrue(MimeType.TryParse(mimeString.AsMemory(), out MimeType mime));

        MimeTypeParameter[] arr = mime.Parameters().ToArray();
        Assert.AreEqual(1, arr.Length);
        Assert.AreEqual(ab, arr[0].Value.ToString(), false);
    }


    [TestMethod()]
    public void ToStringTest1()
    {
        string result = new MimeType().ToString();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }


    [TestMethod()]
    public void ToStringTest2()
    {
        const string input = "text/plain";
        Assert.IsTrue(MimeType.TryParse(input, out MimeType media));
        string result = media.ToString();

        Assert.IsNotNull(result);
        Assert.AreEqual(input, result);
    }


    [TestMethod]
    public void ToStringTest3()
    {
        Assert.IsTrue(MimeType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeType inetMedia));

        Assert.AreEqual("text/plain; charset=iso-8859-1", inetMedia.ToString());
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AppendToTest1()
    {
        Assert.IsTrue(MimeType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeType inetMedia));
        inetMedia.AppendTo(null!);
    }


    [TestMethod]
    public void CloneTest1()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=US-ASCII", out MimeType media1));
        ICloneable o1 = media1;

        object o2 = o1.Clone();

        Assert.IsTrue(o1.Equals(o2));
    }

    [TestMethod]
    public void CloneTest2()
    {
        var o = (MimeType)MimeType.Empty.Clone();
        Assert.IsTrue(o.IsEmpty);
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
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=us-ascii", out MimeType media1));
        Assert.IsTrue(MimeType.TryParse("text/plain", out MimeType media2));

        Assert.IsTrue(media1 == media2);
        Assert.IsFalse(media1 != media2);

        Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest4()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1", out MimeType media1));
        Assert.IsTrue(MimeType.TryParse("text/plain", out MimeType media2));

        Assert.IsTrue(media1 != media2);
        Assert.IsFalse(media1 == media2);

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest5()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1", out MimeType media1));
        Assert.IsTrue(MimeType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1", out MimeType media2));

        Assert.IsTrue(media1 == media2);
        Assert.IsFalse(media1 != media2);

        Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest6()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1;other=value", out MimeType media1));
        Assert.IsTrue(MimeType.TryParse("text/plain;charset=iso-8859-1;OTHER=VALUE", out MimeType media2));

        Assert.IsTrue(media1 != media2);
        Assert.IsFalse(media1 == media2);

        Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
    }

    [TestMethod]
    public void EqualsTest7()
    {
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1", out MimeType media1));
        Assert.IsTrue(MimeType.TryParse("text/plain", out MimeType media2));

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
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=US-ASCII", out MimeType media1));
        Assert.IsTrue(MimeType.TryParse("text/plain", out MimeType media2));

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
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=US-ASCII", out MimeType media1));
        object o1 = media1;
        Assert.IsFalse(o1.Equals(42));
    }

    [TestMethod]
    public void EqualsTest10()
    {
        Assert.IsTrue(MimeType.TryParse("application/octet-stream; charset=US-ASCII", out MimeType media1));
        Assert.IsTrue(MimeType.TryParse("application/octet-stream", out MimeType media2));

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
        Assert.IsTrue(MimeType.TryParse("text/plain; charset=US-ASCII", out MimeType media1));
        Assert.IsTrue(MimeType.TryParse("application/octet-stream", out MimeType media2));

        Assert.IsTrue(media1.IsTextPlain);
        Assert.IsFalse(media2.IsTextPlain);

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

        Assert.IsTrue(MimeType.Parse(media1) == MimeType.Parse(media2) &&
        MimeType.Parse(media2) != MimeType.Parse(media3) &&
        MimeType.Parse(media2) != MimeType.Parse(media4) &&
        MimeType.Parse(media2).Equals(MimeType.Parse(media4), ignoreParameters: true) &&
        MimeType.Parse(media4) == MimeType.Parse(media5) &&
        MimeType.Parse(media4) != MimeType.Parse(media6) &&
        MimeType.Parse(media6) == MimeType.Parse(media7) &&
        MimeType.Parse(media6) != MimeType.Parse(media8));
        
    }

    [TestMethod]
    public void RemoveUrlEncodingTest1()
    {
        Assert.IsTrue(MimeType.TryParse("application/octet-stream; para*=utf-8'de'Hallo%20Folker", out MimeType media1));

        MimeTypeParameter para = media1.Parameters().First();
        Assert.AreEqual(para.Value.ToString(), "Hallo Folker");
        Assert.AreEqual(para.Language.ToString(), "de");
        Assert.AreEqual(para.Key.ToString(), "para");
    }

    [TestMethod]
    public void ParseParametersTest1()
    {
        string input = "application/x-stuff;" + Environment.NewLine +
                        "title*1*=us-ascii'en'This%20is%20even%20more%20;" + Environment.NewLine +
                        "title*2*=%2A%2A%2Afun%2A%2A%2A%20;" + Environment.NewLine +
                        "title*3=\"isn't it!\"";

        Assert.IsTrue(MimeType.TryParse(input, out MimeType mimeType));
        Assert.AreEqual(1, mimeType.Parameters().Count());
        MimeTypeParameter param = mimeType.Parameters().First();

        Assert.AreEqual("title", param.Key.ToString());
        Assert.AreEqual("en", param.Language.ToString());
        Assert.IsTrue(param.Value.EndsWith("isn't it!".AsSpan()));

    }

    [TestMethod]
    public void ParseParametersTest2()
    {
        const string input = "application/x-stuff; param=\"directory\\\\file.text\"";

        Assert.IsTrue(MimeType.TryParse(input, out MimeType mimeType));
        Assert.AreEqual(1, mimeType.Parameters().Count());
        MimeTypeParameter param = mimeType.Parameters().First();

        Assert.AreEqual("param", param.Key.ToString());
        Assert.AreEqual(@"directory\file.text", param.Value.ToString());

        Assert.AreEqual(input, mimeType.ToString());
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
    [DataRow("nixda/nüschgefunden", ".bin")]
    public void GetFileTypeExtensionTest1(string mime, string expected)
        => Assert.AreEqual(expected, MimeType.GetFileTypeExtension(mime));

    [TestMethod]
    public void GetFileTypeExtensionTest2()
        => Assert.AreEqual(".bin", MimeType.GetFileTypeExtension(null));

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
        => Assert.AreEqual(expected, MimeType.FromFileTypeExtension(fileTypeExtension).ToString());

    [TestMethod]
    public void FromFileTypeExtensionTest2()
        => Assert.AreEqual("application/octet-stream", MimeType.FromFileTypeExtension("").ToString());

    [TestMethod]
    public void FromFileTypeExtensionTest3()
        => Assert.AreEqual("application/octet-stream", MimeType.FromFileTypeExtension("".AsSpan()).ToString());

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FromFileTypeExtensionTest4()
        => _ = MimeType.FromFileTypeExtension((string)null!);

    [TestMethod]
    public void BuildAndParseTest1()
    {
        const string mediaType = "application";
        const string subType = "x-stuff";

        var dic = new ParameterModelDictionary()
        {
            new ParameterModel("first-parameter",
            "This is a very long parameter, which will be wrapped according to RFC 2184." +
            Environment.NewLine +
            "It contains also a few Non-ASCII-Characters: \u00E4\u00D6\u00DF.", "en"),
            new ParameterModel("second-parameter", "Parameter with  \\, = and \".")
        };

        var mimeType1 = new MimeType(mediaType, subType, dic);
        string s = mimeType1.ToString(FormattingOptions.LineWrapping | FormattingOptions.Default);

        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
        Assert.AreNotEqual(1, s.GetLinesCount());

        var mimeType2 = MimeType.Parse(s);

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


        var mimeType1 = MimeType.Parse(mimeString);
        string s = mimeType1.ToString(FormattingOptions.LineWrapping | FormattingOptions.Default, 10);

        Assert.IsNotNull(s);
        Assert.AreNotEqual(0, s.Length);
        Assert.AreNotEqual(1, s.GetLinesCount());

        var mimeType2 = MimeType.Parse(s);

        Assert.AreEqual(mediaType, mimeType2.MediaType.ToString(), false);
        Assert.AreEqual(subType, mimeType2.SubType.ToString(), false);

        Assert.AreEqual(1, mimeType2.Parameters().Count());
    }

    

}