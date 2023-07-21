using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class ParameterModelTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MimeTypeParameterModelTest1() => _ = new ParameterModel(null!, "something");

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest2() => _ = new ParameterModel("", "something");

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest3() => _ = new ParameterModel("äöü%@:", "something");

    [DataTestMethod]
    [DataRow("äü")]
    [DataRow("{!")]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest4(string input) => _ = new ParameterModel("key", "something", input);


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest5() => _ = new ParameterModel("key", "something", new string('a', 256));


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest6() => _ = new ParameterModel(new string('a', 5000), "something");

    //[TestMethod]
    //public void IsEmptyTest1() => Assert.IsTrue(new ParameterModel().IsEmpty);

    //[TestMethod]
    //public void IsEmptyTest2() => Assert.IsFalse(new ParameterModel("key", null).IsEmpty);

}
