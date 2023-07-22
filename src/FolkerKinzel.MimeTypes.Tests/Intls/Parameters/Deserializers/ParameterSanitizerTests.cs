using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers.Tests;

[TestClass]
public class ParameterSanitizerTests
{
    [TestMethod]
    public void RepairParameterStringTest1()
    {
        var value = "".AsMemory();
        var sani = new ParameterSanitizer();

        Assert.IsFalse(sani.RepairParameterString(ref value));
    }

    [TestMethod]
    public void RepairParameterStringTest2()
    {
        var value = "(Comm=ent".AsMemory();
        var sani = new ParameterSanitizer();

        Assert.IsFalse(sani.RepairParameterString(ref value));
    }

    [TestMethod]
    public void RepairParameterStringTest3()
    {
        var value = "(Com\\)m=ent)".AsMemory();
        var sani = new ParameterSanitizer();

        Assert.IsFalse(sani.RepairParameterString(ref value));
    }

    [TestMethod]
    public void RepairParameterStringTest4()
    {
        var value = "key=va\\(lue)".AsMemory();
        var sani = new ParameterSanitizer();

        Assert.IsFalse(sani.RepairParameterString(ref value));
    }
}


