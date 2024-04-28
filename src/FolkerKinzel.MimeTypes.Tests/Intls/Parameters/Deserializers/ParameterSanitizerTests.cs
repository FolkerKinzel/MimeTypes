namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers.Tests;

[TestClass]
public class ParameterSanitizerTests
{
    [TestMethod]
    public void RepairParameterStringTest1()
    {
        ReadOnlyMemory<char> value = "".AsMemory();
        var sani = new ParameterSanitizer();

        Assert.IsFalse(sani.RepairParameterString(ref value));
    }

    [TestMethod]
    public void RepairParameterStringTest2()
    {
        ReadOnlyMemory<char> value = "(Comm=ent".AsMemory();
        var sani = new ParameterSanitizer();

        Assert.IsFalse(sani.RepairParameterString(ref value));
    }

    [TestMethod]
    public void RepairParameterStringTest3()
    {
        ReadOnlyMemory<char> value = "(Com\\)m=ent)".AsMemory();
        var sani = new ParameterSanitizer();

        Assert.IsFalse(sani.RepairParameterString(ref value));
    }

    [TestMethod]
    public void RepairParameterStringTest4()
    {
        ReadOnlyMemory<char> value = "key=va\\(lue)".AsMemory();
        var sani = new ParameterSanitizer();

        Assert.IsFalse(sani.RepairParameterString(ref value));
    }
}


