using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class ParameterIndexesTests
{
    [DataTestMethod]
    [DataRow("*=utf-8'de'", 1)]
    public void IndexesTest1(string input, int keyValueOffset)
    {
        const string key = "key";
        string toParse = key + input;
        var idx = new ParameterIndexes(toParse.AsSpan());

        Assert.AreEqual(key.Length, idx.KeyLength);
        Assert.AreEqual(keyValueOffset, idx.KeyValueOffset);

    }

    [TestMethod]
    public void VerifyTest1()
    {
        var idx = new ParameterIndexes("".AsSpan());
        Assert.IsFalse(idx.Verify());
    }

    [TestMethod]
    public void VerifyTest2()
    {
        var idx = new ParameterIndexes((new string('a', short.MaxValue) + "=value").AsSpan());
        Assert.IsFalse(idx.Verify());
    }


    [TestMethod]
    public void ContainsCharSetAndLanguageTest1()
    {
        var idx = new ParameterIndexes("".AsSpan());
        Assert.IsFalse(idx.IsStarred);
    }
}


