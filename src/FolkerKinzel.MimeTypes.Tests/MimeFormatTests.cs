namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeFormatTests
{
    [TestMethod]
    public void MimeFormatTest1()
    {
        MimeFormats fm = default;
        Assert.AreEqual(MimeFormats.Default, fm);
        fm |= MimeFormats.AvoidSpace;
        Assert.IsTrue(fm.HasFlag(MimeFormats.Default));
        fm = MimeFormats.Default | MimeFormats.Url;
        Assert.IsTrue(fm.HasFlag(MimeFormats.AvoidSpace));
        fm &= ~MimeFormats.AvoidSpace;
        Assert.AreNotEqual(MimeFormats.Url, fm);
        Assert.IsFalse(fm.HasFlag(MimeFormats.Url));
    }
}
