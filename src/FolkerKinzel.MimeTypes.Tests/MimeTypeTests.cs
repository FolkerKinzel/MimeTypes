using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.MimeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

[TestClass()]
public class MimeTypeTests
{
    [DataTestMethod()]
    [DataRow(null, "sub")]
    [DataRow("media", null)]
    [DataRow(null, null)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreateTest1(string? nedia, string? sub) => _ = MimeType.Create(nedia!, sub!);

    [TestMethod]
    public void CreateTest2()
    {
        MimeTypeInfo mime1 = MimeType.Create("a", "b")
                                   .AppendParameter("c", "d", "en")
                                   .AppendParameter("e", "f")
                                   .AsInfo();

        MimeTypeInfo mime2 = MimeType.Create(in mime1).AsInfo();

        Assert.AreEqual(mime1.ToString(), mime2.ToString());
    }

    [TestMethod()]
    public void ClearParametersTest1()
    {
        MimeType builder = MimeType.Create("text", "xml")
                   .AppendParameter("charset", "utf-8")
                   .AppendParameter("charset", "UTF-8");

        MimeTypeInfo mime = builder.AsInfo();
        Assert.AreEqual(1, mime.Parameters().Count());
        builder.ClearParameters();
        mime = builder.AsInfo();
        Assert.AreEqual(0, mime.Parameters().Count());
    }

    [TestMethod()]
    public void ClearParametersTest2()
    {
        MimeType builder = MimeType.Create("text", "xml").ClearParameters();
        Assert.IsNotNull(builder);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RemoveParametersTest1() => MimeType.Create("x", "y").RemoveParameter(null!);

    [TestMethod]
    public void RemoveParametersTest2() => MimeType.Create("x", "y").RemoveParameter("nichda").RemoveParameter("");

    [TestMethod]
    public void RemoveParametersTest3()
    {
        MimeTypeInfo mime = MimeType.Create("x", "y")
            .AppendParameter("key1", "val")
            .AppendParameter("key2", "val", "en")
            .RemoveParameter("key1")
            .AsInfo();

        MimeTypeParameterInfo[] paras = mime.Parameters().ToArray();
        Assert.AreEqual(1, paras.Length);
        Assert.AreEqual("key2", paras[0].Key.ToString());
    }



    //[TestMethod()]
    //public void BuildTest()
    //{
    //    Assert.Fail();
    //}
}