namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Tests;

[TestClass]
public class MimeFormatsNormalizerTests
{
    [TestMethod]
    public void MimeFormatNormalizerTest1()
    {
        MimeFormats fm = MimeFormats.IgnoreParameters | MimeFormats.Url;
        Assert.AreNotEqual(MimeFormats.IgnoreParameters, fm);
        Assert.AreNotEqual(MimeFormats.Url, fm);
        fm = fm.Normalize();
        Assert.AreEqual(MimeFormats.IgnoreParameters, fm);
    }

    [TestMethod]
    public void MimeFormatNormalizerTest2()
    {
        MimeFormats fm = MimeFormats.IgnoreParameters | MimeFormats.LineWrapping;
        Assert.AreNotEqual(MimeFormats.IgnoreParameters, fm);
        Assert.AreNotEqual(MimeFormats.LineWrapping, fm);
        fm = fm.Normalize();
        Assert.AreEqual(MimeFormats.IgnoreParameters, fm);
    }

    [TestMethod]
    public void MimeFormatNormalizerTest3()
    {
        MimeFormats fm = MimeFormats.Url | MimeFormats.LineWrapping;
        Assert.AreNotEqual(MimeFormats.Url, fm);
        Assert.AreNotEqual(MimeFormats.LineWrapping, fm);
        fm = fm.Normalize();
        Assert.AreEqual(MimeFormats.Url, fm);
    }


    [TestMethod]
    public void MimeFormatNormalizerTest4()
    {
        MimeFormats fm = MimeFormats.Url & ~MimeFormats.AvoidSpace;
        Assert.AreNotEqual(MimeFormats.Url, fm);
        Assert.AreNotEqual(MimeFormats.Default, fm);
        fm = fm.Normalize();
        Assert.AreEqual(MimeFormats.Default, fm);
    }



}


