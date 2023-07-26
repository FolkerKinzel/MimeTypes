using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Creations.Tests;


[TestClass]
public class ParameterModelTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MimeTypeParameterModelTest1() => _ = new MimeTypeParameter(null!, "something", null);

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest2() => _ = new MimeTypeParameter("", "something", null);

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest3() => _ = new MimeTypeParameter("äöü%@:", "something", null);

    [DataTestMethod]
    [DataRow("äü")]
    [DataRow("{!")]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest4(string input) => _ = new MimeTypeParameter("key", "something", input);


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest5() => _ = new MimeTypeParameter("key", "something", new string('a', 256));


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void MimeTypeParameterModelTest6() => _ = new MimeTypeParameter(new string('a', 5000), "something", null);

    //[TestMethod]
    //public void IsEmptyTest1() => Assert.IsTrue(new ParameterModel().IsEmpty);

    //[TestMethod]
    //public void IsEmptyTest2() => Assert.IsFalse(new ParameterModel("key", null).IsEmpty);

}
