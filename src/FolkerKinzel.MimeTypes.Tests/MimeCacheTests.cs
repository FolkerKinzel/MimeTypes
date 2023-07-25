using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeCacheTests
{
    [TestMethod]
    public void CapacityTest1()
    {
        MimeCache.Clear();
        string ext = MimeConverter.ToFileTypeExtension("image/jpeg");
        Assert.AreEqual(".jpg", ext);
        string mime = MimeConverter.FromFileName(".odt".AsSpan());
        StringAssert.StartsWith(mime, "application/");
        Assert.IsTrue(MimeCache.Capacity >= MimeCache.DefaultCapacity);
        int capacity = MimeCache.Capacity;
        MimeCache.EnlargeCapacity(capacity + 100);
        Assert.IsTrue(MimeCache.Capacity > capacity);
        MimeCache.EnlargeCapacity(MimeCache.DefaultCapacity);
        Assert.IsTrue(MimeCache.Capacity > MimeCache.DefaultCapacity);
    }

}
