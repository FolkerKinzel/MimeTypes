using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers.Tests;

[TestClass]
public class ParameterValueDecoderTests
{
    [TestMethod]
    public void TryDecodeValueTest1()
    {
        const string input = "key*=utf-8'en'%EF%EE";

        var idx = new ParameterIndexes(input.AsSpan());
        ReadOnlyMemory<char> mem = input.AsMemory();
        Assert.IsFalse(ParameterValueDecoder.TryDecodeValue(in idx, ref mem));
    }
}


