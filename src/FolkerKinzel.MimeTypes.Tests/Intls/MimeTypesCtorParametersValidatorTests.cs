namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class MimeTypesCtorParametersValidatorTests
{
    [TestMethod]
    public void ValidateSubTypeTest1()
        => Assert.ThrowsExactly<ArgumentException>(
            () => MimeTypeCtorParametersValidator.Validate("image", new string('a', short.MaxValue + 1)));
}


