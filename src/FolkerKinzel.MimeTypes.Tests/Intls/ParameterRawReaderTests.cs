using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class ParameterRawReaderTests
{
    [TestMethod]
    public void GetNextParameterValidatorIndexTest1() => 
        Assert.AreEqual(-1, ParameterRawReader.GetNextParameterSeparatorIndex("key==value;".AsSpan()));
}


