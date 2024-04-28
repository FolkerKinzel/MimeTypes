namespace FolkerKinzel.MimeTypes.Intls.Parameters.Creations.Tests;

[TestClass]
public class IetfLanguageTagTests
{
    [TestMethod]
    public void ValidateTest1() => Assert.IsFalse(IetfLanguageTag.Validate("   ".AsSpan()));

    [TestMethod]
    public void ValidateTest2() => Assert.IsFalse(IetfLanguageTag.Validate("".AsSpan()));
}


