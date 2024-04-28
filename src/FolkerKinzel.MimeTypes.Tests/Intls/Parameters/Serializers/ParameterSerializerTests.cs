namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Tests;

[TestClass]
public class ParameterSerializerTests
{
    [TestMethod]
    public void AppendTest1()
    {
        var model = new MimeTypeParameter("key", null, null);

        var sb = new StringBuilder();
        ParameterSerializer.AppendTo(sb, model, false);
        StringAssert.Contains(sb.ToString(), "key=\"\"");
    }

    [TestMethod]
    public void AppendTest2()
    {
        var model = new MimeTypeParameter("key", null, "en");

        var sb = new StringBuilder();
        ParameterSerializer.AppendTo(sb, model, false);
        StringAssert.Contains(sb.ToString(), "key*=utf-8'en'");
    }

    [TestMethod]
    public void AppendTest3()
    {
        var sb = new StringBuilder();
        const string nonAscii = "ä";
        const string ascii = "para";

        ParameterSerializer.AppendTo(sb, new MimeTypeParameter(ascii, nonAscii, null), false);
        Assert.AreNotEqual(0, sb.Length);
        string s = sb.ToString();
        Assert.IsTrue(s.Contains(ascii));
        Assert.IsFalse(s.Contains(nonAscii));
    }

    [TestMethod]
    public void AppendTest4()
    {
        ReadOnlyMemory<char> mem = "x/y; key=".AsMemory();
        Assert.IsTrue(MimeType.TryParse(in mem, out MimeType? mime));
        StringAssert.Contains(mime.Parameters.First().ToString(), "key=\"\"");
    }

    [TestMethod]
    public void AppendTest5()
    {
        ReadOnlyMemory<char> mem = "x/y; key*='en'".AsMemory();
        Assert.IsTrue(MimeType.TryParse(in mem, out MimeType? mime));
        StringAssert.Contains(mime.ToString(), "key*=utf-8'en'");
    }

    [TestMethod]
    public void AppendTest6()
    {
        ReadOnlyMemory<char> mem = ("x/y; key*=''" + Uri.EscapeDataString("äöü")).AsMemory();
        Assert.IsTrue(MimeType.TryParse(in mem, out MimeType? mime));

        StringAssert.Contains(mime.ToString(), "key*=");
    }

    [TestMethod]
    public void AppendTest7()
    {
        const string bla = "x/y; charset=\"BLA\\\"BLA\"";
        ReadOnlyMemory<char> mem = bla.AsMemory();
        Assert.IsTrue(MimeType.TryParse(in mem, out MimeType? mime));
        StringAssert.Contains(mime.ToString(), bla.ToLowerInvariant());
    }
}


