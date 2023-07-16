using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeCacheTests
{
    [TestMethod]
    public void CapacityTest1()
    {
        Assert.AreEqual(MimeCache.DefaultCapacity, MimeCache.Capacity);
        MimeCache.EnlargeCapacity(MimeCache.Capacity + 100);
        Assert.IsTrue(MimeCache.Capacity > MimeCache.DefaultCapacity);
        MimeCache.EnlargeCapacity(MimeCache.DefaultCapacity);
        Assert.IsTrue(MimeCache.Capacity > MimeCache.DefaultCapacity);
    }

}
