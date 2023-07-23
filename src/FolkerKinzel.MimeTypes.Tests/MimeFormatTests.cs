using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeFormatTests
{
    [TestMethod]
    public void MimeFormatTest1()
    {
        MimeFormat fm = default;
        Assert.AreEqual(MimeFormat.Default, fm);
        fm |= MimeFormat.AvoidSpace;
        Assert.IsTrue(fm.HasFlag(MimeFormat.Default));
        fm = MimeFormat.Default | MimeFormat.Url;
        Assert.IsTrue(fm.HasFlag(MimeFormat.AvoidSpace));
        fm &= ~MimeFormat.AvoidSpace;
        Assert.AreNotEqual(MimeFormat.Url, fm);
        Assert.IsFalse(fm.HasFlag(MimeFormat.Url));
    }
}
