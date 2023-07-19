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
}


