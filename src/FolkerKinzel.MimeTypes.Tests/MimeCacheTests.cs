using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeCacheTests
{
    [TestMethod]
    public void CapacityTest1()
    {
        MimeCache.Clear();
        string ext = MimeType.ToFileExtension("image/jpeg");
        Assert.AreEqual(".jpg", ext);
        string mime = MimeType.FromFileExtension(".odt".AsSpan());
        StringAssert.StartsWith(mime, "application/");
        Assert.IsTrue(MimeCache.Capacity >= MimeCache.DefaultCapacity);
        int capacity = MimeCache.Capacity;
        MimeCache.EnlargeCapacity(capacity + 100);
        Assert.IsTrue(MimeCache.Capacity > capacity);
        MimeCache.EnlargeCapacity(MimeCache.DefaultCapacity);
        Assert.IsTrue(MimeCache.Capacity > MimeCache.DefaultCapacity);
    }

}
