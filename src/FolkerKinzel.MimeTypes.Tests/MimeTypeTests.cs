namespace FolkerKinzel.MimeTypes.Tests;

[TestClass()]
public class MimeTypeTests
{
    [TestMethod()]
    [DataRow(null, "sub")]
    [DataRow("media", null)]
    [DataRow(null, null)]
    public void CreateTest1(string? media, string? sub) 
        => _ = Assert.ThrowsExactly<ArgumentNullException>(() => MimeType.Create(media!, sub!));

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
    public void CreateTest3() 
        => _ = Assert.ThrowsExactly<ArgumentException>(() => MimeType.Create(new MimeTypeInfo()));

    [TestMethod]
    public void CreateTest4()
    {
        const string input = "key/value; äöü=äöü";

        Assert.IsTrue(MimeTypeInfo.TryParse(input, out MimeTypeInfo mimeTypeInfo));

        var mime = MimeType.Create(in mimeTypeInfo);
        Assert.IsEmpty(mime.Parameters);
    }


    [TestMethod]
    public void ParseTest1()
    {
        ReadOnlyMemory<char> mem = "image/jpeg".AsMemory();
        Assert.IsNotNull(MimeType.Parse(in mem));
    }

    [TestMethod]
    public void TryParseTest1() => Assert.IsFalse(MimeType.TryParse("blabla", out _));

    [TestMethod]
    public void TryParseTest1b() => Assert.IsFalse(MimeType.TryParse("blabla".AsMemory(), out _));

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
    public void TryParseTest2() => Assert.IsFalse(MimeType.TryParse(null, out _));

    [TestMethod]
    public void AppendParameterTest1()
        => _ = Assert.ThrowsExactly<ArgumentNullException>(
            () => MimeType.Create("image", "png").AppendParameter(null!, "something", null));

    [TestMethod]
    public void AppendParameterTest2() 
        => _ = Assert.ThrowsExactly<ArgumentException>(
            () => MimeType.Create("image", "png").AppendParameter("", "something", null));

    [TestMethod]
    public void AppendParameterTest3()
        => _ = Assert.ThrowsExactly<ArgumentException>(
            () => MimeType.Create("image", "png").AppendParameter("äöü%@:", "something", null));

    [TestMethod]
    public void AppendParameterTest4()
        => _ = Assert.ThrowsExactly<ArgumentException>(
            () => MimeType.Create("image", "png").AppendParameter(new string('a', 5000), "something", null));


    [TestMethod]
    [DataRow("äü")]
    [DataRow("{!")]
    public void AppendParameterTest5(string input)
        => _ = Assert.ThrowsExactly<ArgumentException>(
            () => MimeType.Create("image", "png").AppendParameter("key", "something", input));

    [TestMethod]
    public void AppendParameterTest6()
        => _ = Assert.ThrowsExactly<ArgumentException>(
            () => MimeType.Create("image", "png").AppendParameter("key", "something", new string('a', 256)));

    [TestMethod()]
    public void ClearParametersTest1()
    {
        MimeType builder = MimeType.Create("text", "xml")
                   .AppendParameter("charset", "utf-8")
                   .AppendParameter("charset", "UTF-8");

        MimeTypeInfo mime = builder.AsInfo();
        Assert.HasCount(1, mime.Parameters());
        builder.ClearParameters();
        mime = builder.AsInfo();
        Assert.IsEmpty(mime.Parameters());
    }

    [TestMethod()]
    public void ClearParametersTest2()
    {
        MimeType builder = MimeType.Create("text", "xml").ClearParameters();
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    public void RemoveParametersTest1()
        => _ = Assert.ThrowsExactly<ArgumentNullException>(
            () => MimeType.Create("x", "y").RemoveParameter(null!));

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

        MimeTypeParameterInfo[] paras = [.. mime.Parameters()];
        Assert.HasCount(1, paras);
        Assert.AreEqual("key2", paras[0].Key.ToString());
    }

    [TestMethod]
    public void AppendToTest1()
        => _ = Assert.ThrowsExactly<ArgumentNullException>(() => MimeType.Create("x", "y").AppendTo(null!));


    [TestMethod]
    public void AppendToTest2()
        => _ = MimeType.Create("x", "y")
                       .AppendParameter("a", "b")
                       .AppendTo(new StringBuilder(), MimeFormats.LineWrapping, -42);

    [TestMethod]
    public void AppendToTest3()
    {
        var mime = MimeType.Create("application", 
                                   "x-veeeeeeeeeeeeryyyyyyyyyyyyyyveryyyyyyyyyyyyyyyyyylooooooooong");
        var builder = new StringBuilder("Content-type: ");
        string result = mime.AppendTo(builder, options: MimeFormats.LineWrapping).ToString();
        Assert.AreEqual(2, result.GetLinesCount());
    }

    [TestMethod]
    public void ToStringTest1()
    {
        MimeType mime = MimeType.Create("application", "x-stuff")
            .AppendParameter("keyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", 
                             "valueeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");

        string s = mime.ToString(options: MimeFormats.LineWrapping, lineLength: 10);
        Assert.AreNotEqual(0, s.Length);
    }

    [TestMethod]
    public void ToStringTest2() => Assert.AreEqual("image/png", MimeType.Create("image", "png").ToString());


    [TestMethod]
    public void ToStringTest3()
    {
        string result = MimeType.Create("application", "x-stuff")
                                .AppendParameter("mypara", "UPPERCASE")
                                .ToString();
        Assert.Contains("UPPERCASE", result, StringComparison.Ordinal);
    }

    [TestMethod]
    public void EqualsTest1()
    {
        string media1 = "text/plain; charset=iso-8859-1";
        string media2 = "TEXT/PLAIN; CHARSET=ISO-8859-1";

        var mediaType1 = MimeType.Parse(media1);
        var mediaType2 = MimeType.Parse(media2);

        Assert.IsTrue(mediaType1.IsText);
        Assert.IsTrue(mediaType1.IsTextPlain);
        Assert.IsFalse(mediaType1.IsOctetStream);
        Assert.IsTrue(mediaType2.IsText);
        Assert.IsTrue(mediaType2.IsTextPlain);
        Assert.IsFalse(mediaType2.IsOctetStream);

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
        Assert.IsTrue(MimeType.TryParse("APPLICATION/Octet-Stream", out MimeType? media2));

        Assert.IsTrue(media1.IsOctetStream);
        Assert.IsFalse(media1.IsText);
        Assert.IsFalse(media1.IsTextPlain);
        Assert.IsTrue(media2.IsOctetStream);
        Assert.IsFalse(media2.IsText);
        Assert.IsFalse(media2.IsTextPlain);

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
    public void EqualsTest13() => Assert.IsFalse(MimeType.Create("x", "y").Equals(42));

    [TestMethod]
    public void EqualsTest14() => Assert.IsFalse(MimeType.Create("x", "y").Equals((object)MimeType.Create("a", "b")));


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


    [TestMethod]
    public void AsInfoTest1()
    {
        MimeTypeInfo info = MimeType.Create("application", "x-stuff").AppendParameter("key", "value").ClearParameters().AsInfo();
        Assert.IsEmpty(info.Parameters());
    }

    [TestMethod]
    public void AsInfoTest2()
    {
        MimeTypeInfo info = MimeType.Create("application", "x-stuff").AsInfo();
        Assert.IsEmpty(info.Parameters());
    }

    [TestMethod]
    public void FromFileNameTest1()
    {
        var mime = MimeType.FromFileName("test.jpg");
        Assert.AreEqual("image", mime.MediaType);
        Assert.AreEqual("jpeg", mime.SubType);
        Assert.IsEmpty(mime.Parameters);
    }

    [TestMethod]
    public void FromFileNameTest2()
    {
        var mime = MimeType.FromFileName("test.jpg".AsSpan());
        Assert.AreEqual("image", mime.MediaType);
        Assert.AreEqual("jpeg", mime.SubType);
        Assert.IsEmpty(mime.Parameters);
    }
}