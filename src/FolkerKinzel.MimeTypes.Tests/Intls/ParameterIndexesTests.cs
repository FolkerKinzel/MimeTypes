using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class ParameterIndexesTests
{
    [TestMethod]
    public void VerifyTest1()
    {
        var idx = new ParameterIndexes("".AsSpan());
        Assert.IsFalse(idx.Verify());
    }

    [TestMethod]
    public void VerifyTest2()
    {
        var idx = new ParameterIndexes("".AsSpan());
        idx.KeyLength = short.MaxValue;
        Assert.IsFalse(idx.Verify());
    }


    [TestMethod]
    public void ContainsCharSetAndLanguageTest1()
    {
        var idx = new ParameterIndexes("".AsSpan());
        Assert.IsFalse(idx.ContainsCharSetAndLanguage());
    }
}


