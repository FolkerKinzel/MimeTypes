using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeTypeParameterModelTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MimeTypeParameterModelTest1() => _ = new MimeTypeParameterModel(null!, "something");

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest2() => _ = new MimeTypeParameterModel("", "something");

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest3() => _ = new MimeTypeParameterModel("äöü%@:", "something");

    [DataTestMethod]
    [DataRow("äü")]
    [DataRow("{!")]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest4(string input) => _ = new MimeTypeParameterModel("key", "something", input);


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest5() => _ = new MimeTypeParameterModel("key", "something", new string('a', 256));


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest4() => _ = new MimeTypeParameterModel(new string('a', 5000), "something");

    [TestMethod]
    public void IsEmptyTest1() => Assert.IsTrue(new MimeTypeParameterModel().IsEmpty);

    [TestMethod]
    public void IsEmptyTest2() => Assert.IsFalse(new MimeTypeParameterModel("key", null).IsEmpty);

}
