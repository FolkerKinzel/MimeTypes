using FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers.Tests;

[TestClass]
public class ParameterRawReaderTests
{
    [TestMethod]
    public void GetNextParameterSeparatorIndexTest1() =>
        Assert.AreEqual(-1, ParameterRawReader.GetNextParameterSeparatorIndex("key==value;".AsSpan()));


    [TestMethod]
    public void GetNextParameterSeparatorIndexTest2()
    {
        ReadOnlySpan<char> span = "key=\"val\\\" ".AsSpan();
        Assert.AreEqual(-1, ParameterRawReader.GetNextParameterSeparatorIndex(span));

    }

    [TestMethod]
    public void GetNextParameterSeparatorIndexTest3()
    {
        ReadOnlySpan<char> span = "key=\"val\\\" ;".AsSpan();
        Assert.AreEqual(span.Length - 1, ParameterRawReader.GetNextParameterSeparatorIndex(span));

    }
}


