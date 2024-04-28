namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Tests;

[TestClass]
public class MimeFormatsNormalizerTests
{
    [TestMethod]
    public void MimeFormatNormalizerTest1()
    {
        MimeFormats fm = MimeFormats.IgnoreParameters | MimeFormats.Url;
        Assert.AreNotEqual(fm, MimeFormats.IgnoreParameters);
        Assert.AreNotEqual(fm, MimeFormats.Url);
        fm = fm.Normalize();
        Assert.AreEqual(fm, MimeFormats.IgnoreParameters);
    }

    [TestMethod]
    public void MimeFormatNormalizerTest2()
    {
        MimeFormats fm = MimeFormats.IgnoreParameters | MimeFormats.LineWrapping;
        Assert.AreNotEqual(fm, MimeFormats.IgnoreParameters);
        Assert.AreNotEqual(fm, MimeFormats.LineWrapping);
        fm = fm.Normalize();
        Assert.AreEqual(fm, MimeFormats.IgnoreParameters);
    }

    [TestMethod]
    public void MimeFormatNormalizerTest3()
    {
        MimeFormats fm = MimeFormats.Url | MimeFormats.LineWrapping;
        Assert.AreNotEqual(fm, MimeFormats.Url);
        Assert.AreNotEqual(fm, MimeFormats.LineWrapping);
        fm = fm.Normalize();
        Assert.AreEqual(fm, MimeFormats.Url);
    }


    [TestMethod]
    public void MimeFormatNormalizerTest4()
    {
        MimeFormats fm = MimeFormats.Url & ~MimeFormats.AvoidSpace;
        Assert.AreNotEqual(fm, MimeFormats.Url);
        Assert.AreNotEqual(fm, MimeFormats.Default);
        fm = fm.Normalize();
        Assert.AreEqual(fm, MimeFormats.Default);
    }



}


