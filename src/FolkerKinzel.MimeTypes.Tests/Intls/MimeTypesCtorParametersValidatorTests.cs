using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class MimeTypesCtorParametersValidatorTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateSubTypeTest1() => 
        MimeTypeCtorParametersValidator.Validate("image", new string('a', short.MaxValue + 1));
}


