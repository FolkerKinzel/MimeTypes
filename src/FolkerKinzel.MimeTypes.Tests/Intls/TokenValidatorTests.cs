namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class TokenValidatorTests
{
    private const string ALLOWED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~!{}-.$";

    [DataTestMethod]
    [DataRow("%")]
    [DataRow("*")]
    [DataRow("\'")]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateTokenParameterTest1(string token) => token.ValidateTokenParameter(nameof(token), true);

    [TestMethod]
    public void ValidateTokenParameterTest1b() => ALLOWED_CHARS.ValidateTokenParameter("x", true);


    [DataTestMethod]
    [DataRow("%")]
    [DataRow("*")]
    [DataRow("\'")]
    [DataRow(ALLOWED_CHARS)]
    public void ValidateTokenParameterTest2(string token) => token.ValidateTokenParameter(nameof(token), false);

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ValidateTokenParameterTest3() => TokenValidator.ValidateTokenParameter(null!, "x", true);

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ValidateTokenParameterTest4() => TokenValidator.ValidateTokenParameter(null!, "x", false);

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("a b")]
    [DataRow("\"")]
    [DataRow("[")]
    [DataRow("\\")]
    [DataRow("]")]
    [DataRow("/")]
    [DataRow(",")]
    [DataRow("(")]
    [DataRow(")")]
    [DataRow(">")]
    [DataRow("<")]
    [DataRow("\r")]
    [DataRow("\n")]
    [DataRow("@")]
    [DataRow(",")]
    [DataRow(";")]
    [DataRow(":")]
    [DataRow("?")]
    [DataRow("=")]
    [DataRow("ä")]
    [ExpectedException(typeof(ArgumentException))]
    public void ValidateTokenParameterTest5(string token) => token.ValidateTokenParameter("x", false);
}


