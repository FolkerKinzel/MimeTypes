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
}
