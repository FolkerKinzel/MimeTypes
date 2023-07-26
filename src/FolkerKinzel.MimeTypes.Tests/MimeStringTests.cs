using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeStringTests
{
    [TestMethod]
    public void FromFileNameTest1()
    {
#if NET461 || NETSTANDARD2_0
        const string expected = MimeType.Default;
#else
        const string expected = "text/plain";
#endif

        Assert.AreEqual(expected, MimeString.FromFileName("te<st.txt"));
    }

    [TestMethod]
    public void FromFileNameTest2()
    {
        string mime1 = MimeString.FromFileName(".png");

        string mime2 = MimeString.FromFileName("png");
        string mime3 = MimeString.FromFileName(".PNG");
        string mime4 = MimeString.FromFileName("PNG");
        string mime5 = MimeString.FromFileName("picture.png");
        string mime6 = MimeString.FromFileName("..\\picture.png");

        Assert.AreEqual("image/png", mime1);

        string[] arr = new string[] { mime2, mime3, mime4, mime5, mime6};
        Assert.IsTrue(arr.All(x => x.Equals(mime1)));

    }
}
