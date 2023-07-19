using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class ParameterSplitterTests
{
    [TestMethod]
    public void SplitParameterTest1()
    {
        ReadOnlyMemory<char> mem = "keyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy=utf-8'en'Very%20very%20very%20very%20looooooooooooooooooooong%20text\"".AsMemory();
        Assert.IsTrue(MimeTypeParameter.TryParse(true, ref mem, out MimeTypeParameter param));

        var input = new StringBuilder();
        param.AppendTo(input);
        string s = "";
        foreach(StringBuilder sb in ParameterSplitter.SplitParameter(param, input, -42))
        {
            s += sb.ToString() + Environment.NewLine;
        }
    }


    [TestMethod]
    public void SplitParameterTest2() 
    {
        const string value = "äääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääääää";
        string s = "application/x-stuff;key*=utf-8'de'" + Uri.UnescapeDataString(value);
        
        Assert.IsTrue(MimeType.TryParse(s, out MimeType mime));
        var para = mime.Parameters().First();
        Assert.AreEqual(value, para.Value.ToString(), false);

        s = mime.ToString(FormattingOptions.IncludeParameters |FormattingOptions.LineWrapping);

        Assert.IsTrue(MimeType.TryParse(s, out mime));
        para = mime.Parameters().First();
        Assert.AreEqual(value, para.Value.ToString(), false);
    }


    [TestMethod]
    public void SplitParameterTest3()
    {
        string s = """
                   application/x-stuff;
                   key*0=\"Masked \\\"1\\\"\";
                   key*1=\"Masked \\\"2\\\"\";
                   key*2=\"Masked \\\"3\\\"\";
                   """;
        Assert.IsTrue(MimeType.TryParse(s, out MimeType mime));
        var para = mime.Parameters().First();
        //Assert.AreEqual(value, para.Value.ToString(), false);

        s = mime.ToString(FormattingOptions.IncludeParameters | FormattingOptions.LineWrapping);

        Assert.IsTrue(MimeType.TryParse(s, out mime));
        para = mime.Parameters().First();
        //Assert.AreEqual(value, para.Value.ToString(), false);
    }
}


