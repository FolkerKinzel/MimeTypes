using FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers.Tests;

[TestClass]
public class ParameterRawReaderTests
{
    [TestMethod]
    public void GetNextParameterValidatorIndexTest1() =>
        Assert.AreEqual(-1, ParameterRawReader.GetNextParameterSeparatorIndex("key==value;".AsSpan()));
}


