using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeCacheTests
{
    [TestMethod]
    public void CapacityTest1()
    {
        MimeCache.Clear();
        string ext = MimeString.ToFileTypeExtension("image/jpeg");
        Assert.AreEqual(".jpg", ext);
        string mime = MimeString.FromFileName(".odt".AsSpan());
        StringAssert.StartsWith(mime, "application/");
        Assert.IsTrue(MimeCache.Capacity >= MimeCache.DefaultCapacity);
        int capacity = MimeCache.Capacity;
        MimeCache.EnlargeCapacity(capacity + 100);
        Assert.IsTrue(MimeCache.Capacity > capacity);
        MimeCache.EnlargeCapacity(MimeCache.DefaultCapacity);
        Assert.IsTrue(MimeCache.Capacity > MimeCache.DefaultCapacity);
    }

    [TestMethod]
    public void GetFileTypeExtension1() => Assert.AreEqual("bin", MimeCache.GetFileTypeExtension("abcd", false));

}
