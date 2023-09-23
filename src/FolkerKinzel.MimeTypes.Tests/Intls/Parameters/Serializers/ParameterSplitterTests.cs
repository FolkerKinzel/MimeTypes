using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;
using FolkerKinzel.MimeTypes.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

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
        para.AppendTo(input);

        EncodingAction enc = input.Append(para, false);
        string s = "";
        foreach (StringBuilder sb in ParameterSplitter.SplitParameter(para, input, 70, enc))
        {
            s += sb.ToString() + Environment.NewLine;
        }

        Assert.AreNotEqual(1, s.GetLinesCount());   
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


