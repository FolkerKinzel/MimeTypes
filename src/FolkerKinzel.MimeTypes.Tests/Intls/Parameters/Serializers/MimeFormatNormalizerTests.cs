using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Tests;

[TestClass]
public class MimeFormatNormalizerTests
{
    [TestMethod]
    public void MimeFormatNormalizerTest1()
    {
        MimeFormat fm = MimeFormat.IgnoreParameters | MimeFormat.Url;
        Assert.AreNotEqual(fm, MimeFormat.IgnoreParameters);
        Assert.AreNotEqual(fm, MimeFormat.Url);
        fm = fm.Normalize();
        Assert.AreEqual(fm, MimeFormat.IgnoreParameters);
    }

    [TestMethod]
    public void MimeFormatNormalizerTest2()
    {
        MimeFormat fm = MimeFormat.IgnoreParameters | MimeFormat.LineWrapping;
        Assert.AreNotEqual(fm, MimeFormat.IgnoreParameters);
        Assert.AreNotEqual(fm, MimeFormat.LineWrapping);
        fm = fm.Normalize();
        Assert.AreEqual(fm, MimeFormat.IgnoreParameters);
    }

    [TestMethod]
    public void MimeFormatNormalizerTest3()
    {
        MimeFormat fm = MimeFormat.Url | MimeFormat.LineWrapping;
        Assert.AreNotEqual(fm, MimeFormat.Url);
        Assert.AreNotEqual(fm, MimeFormat.LineWrapping);
        fm = fm.Normalize();
        Assert.AreEqual(fm, MimeFormat.Url);
    }


    [TestMethod]
    public void MimeFormatNormalizerTest4()
    {
        MimeFormat fm = MimeFormat.Url & ~MimeFormat.AvoidSpace;
        Assert.AreNotEqual(fm, MimeFormat.Url);
        Assert.AreNotEqual(fm, MimeFormat.Default);
        fm = fm.Normalize();
        Assert.AreEqual(fm, MimeFormat.Default);
    }



}


