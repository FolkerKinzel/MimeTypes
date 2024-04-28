using FolkerKinzel.MimeTypes.Tests;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Tests;

[TestClass]
public class ParameterSplitterTests
{
    [TestMethod]
    public void SplitParameterTest1()
    {
        const string mimeString = "a/b; keyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy*=utf-8'en'Very%20very%20very%20very%20looooooooooooooooooooong%20text\"";
        Assert.IsTrue(MimeType.TryParse(mimeString, out MimeType? mime));

        var input = new StringBuilder();
        MimeTypeParameter para = mime.Parameters.First();

        EncodingAction enc = ParameterSerializer.AppendTo(input, para, false);

        var builder = new StringBuilder();
        ParameterSerializer.SplitParameter(builder, input, 70, para.Key.Length, para.Language?.Length ?? 0, false, enc, 0);

        Assert.AreNotEqual(1, builder.ToString().GetLinesCount());
    }


    [TestMethod]
    public void SplitParameterTest2()
    {
        const string value = "äääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääää";
        string s = "application/x-stuff;key*=utf-8'de'" + Uri.UnescapeDataString(value);

        Assert.IsTrue(MimeType.TryParse(s, out MimeType? mime));
        MimeTypeParameter para = mime.Parameters.First();
        Assert.AreEqual(value, para.Value, false);

        s = mime.ToString(MimeFormats.LineWrapping);

        Assert.IsTrue(MimeType.TryParse(s, out mime));
        para = mime.Parameters.First();
        Assert.AreEqual(value, para.Value, false);
    }


    [TestMethod]
    public void SplitParameterTest3()
    {
        const string value = """
        Masked "1" Masked "2" Masked "3" 
        """;

        string s = """
                   application /x-stuff;
                   key*0="Masked \"1\" ";
                   key*1="Masked \"2\" ";
                   key*2="Masked \"3\" ";
                   """;
        Assert.IsTrue(MimeType.TryParse(s, out MimeType? mime));
        MimeTypeParameter para = mime.Parameters.First();
        Assert.AreEqual(value, para.Value, false);

        s = mime.ToString(MimeFormats.LineWrapping);

        Assert.IsTrue(MimeType.TryParse(s, out mime));
        para = mime.Parameters.First();
        Assert.AreEqual(value, para.Value, false);
    }
}


