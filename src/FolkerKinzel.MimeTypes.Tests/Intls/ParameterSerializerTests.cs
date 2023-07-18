using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class ParameterSerializerTests
{
    [TestMethod]
    public void AppendTest1()
    {
        var model = new MimeTypeParameterModel("key", null);

        var sb = new StringBuilder();
        ParameterSerializer.Append(sb, in model);
        StringAssert.Contains(sb.ToString(), "key=\"\"");
    }

    [TestMethod]
    public void AppendTest2()
    {
        var model = new MimeTypeParameterModel("key", null,"en");

        var sb = new StringBuilder();
        ParameterSerializer.Append(sb, in model);
        StringAssert.Contains(sb.ToString(), "key*='en'");
    }
}


