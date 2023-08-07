using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders.Tests;

[TestClass]
public class ValueBuilderUrlEncodedTests
{
#if NET461 || NETCOREAPP2_1 || NETCOREAPP3_1
    [TestMethod]
    public void BuildUrlEncodedTests()
    {
        var parameter = new MimeTypeParameter("key", new string('ä', 100000), "de");
        var builder = new StringBuilder();
        ValueBuilderUrlEncoded.BuildUrlEncoded(builder, parameter);
        Assert.AreEqual(0, builder.Length);
    }
#endif
}


