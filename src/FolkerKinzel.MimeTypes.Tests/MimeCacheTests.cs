using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeCacheTests
{
    [TestMethod]
    public void CapacityTest1()
    {
        Assert.AreEqual(0,MimeCache.Capacity);
        MimeCache.EnlargeCapacity(100);
        Assert.AreNotEqual(0, MimeCache.Capacity);

    }

}
