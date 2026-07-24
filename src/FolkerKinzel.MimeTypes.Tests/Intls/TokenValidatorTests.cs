namespace FolkerKinzel.MimeTypes.Intls.Tests;

[TestClass]
public class TokenValidatorTests
{
    private const string ALLOWED_CHARS 
        = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~!{}-.$";

    [TestMethod]
    [DataRow("%")]
    [DataRow("*")]
    [DataRow("\'")]
    public void ValidateTokenParameterTest1(string token)
        => Assert.ThrowsExactly<ArgumentException>(
            () => token.ValidateTokenParameter(nameof(token), true));

    [TestMethod]
    public void ValidateTokenParameterTest1b() 
        => ALLOWED_CHARS.ValidateTokenParameter("x", true);


    [TestMethod]
    [DataRow("%")]
    [DataRow("*")]
    [DataRow("\'")]
    [DataRow(ALLOWED_CHARS)]
    public void ValidateTokenParameterTest2(string token)
        => token.ValidateTokenParameter(nameof(token), false);

    [TestMethod]
    public void ValidateTokenParameterTest3()
        => Assert.ThrowsExactly<ArgumentNullException>(
            () => TokenValidator.ValidateTokenParameter(null!, "x", true));

    [TestMethod]
    public void ValidateTokenParameterTest4()
        => Assert.ThrowsExactly<ArgumentNullException>(
            () => TokenValidator.ValidateTokenParameter(null!, "x", false));

    [TestMethod]
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
    [DataRow(";")]
    [DataRow(":")]
    [DataRow("?")]
    [DataRow("=")]
    [DataRow("ä")]
    public void ValidateTokenParameterTest5(string token)
        => Assert.ThrowsExactly<ArgumentException>(
            () => token.ValidateTokenParameter("x", false));
}


