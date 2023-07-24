using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.MimeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass()]
public class MimeTypeBuilderTests
{
    [DataTestMethod()]
    [DataRow(null, "sub")]
    [DataRow("media", null)]
    [DataRow(null, null)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreateTest1(string? nedia, string? sub)
    {
        _ = MimeTypeBuilder.Create(nedia!, sub!);
    }

    //[TestMethod]
    //public void MimeTypeBuilderTest2()
    //{
    //    var builder = new MimeTypeBuilder();
    //    Assert.IsTrue(builder.IsEmpty);
    //    MimeType mime = builder.Build();
    //    Assert.IsTrue(mime.IsEmpty);
    //}

    //[TestMethod()]
    //public void AddParameterTest()
    //{
    //    Assert.Fail();
    //}

    [TestMethod()]
    public void ClearParametersTest1()
    {
        MimeTypeBuilder builder = MimeTypeBuilder.Create("text", "xml")
                   .AppendParameter("charset", "utf-8")
                   .AppendParameter("charset", "UTF-8");

        MimeTypeInfo mime = builder.Build();
        Assert.AreEqual(1, mime.Parameters().Count());
        builder.ClearParameters();
        mime = builder.Build();
        Assert.AreEqual(0, mime.Parameters().Count());
    }

    [TestMethod()]
    public void ClearParametersTest2()
    {
        MimeTypeBuilder builder = MimeTypeBuilder.Create("text", "xml").ClearParameters();
        Assert.IsNotNull(builder);
    }

    //[TestMethod()]
    //public void BuildTest()
    //{
    //    Assert.Fail();
    //}
}