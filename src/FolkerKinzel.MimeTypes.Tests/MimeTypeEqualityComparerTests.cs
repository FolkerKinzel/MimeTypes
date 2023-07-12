using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.MimeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass()]
public class MimeTypeEqualityComparerTests
{
    [TestMethod()]
    public void MimeTypeEqualityComparerTest1()
    {
        var dic = new MimeTypeParameterModelDictionary()
        {
            new MimeTypeParameterModel("para1", "the value")
        };
        var mime1 = new MimeType("image", "png", dic);
        var mime2 = new MimeType("image", "png");

        MimeTypeEqualityComparer cmp1 = MimeTypeEqualityComparer.IgnoreParameters;
        MimeTypeEqualityComparer cmp2 = MimeTypeEqualityComparer.Default;

        Assert.AreEqual(cmp1.GetHashCode(mime1), cmp1.GetHashCode(mime2));
        Assert.AreNotEqual(cmp2.GetHashCode(mime1), cmp2.GetHashCode(mime2));

        Assert.IsTrue(cmp1.Equals(mime1, mime2));
        Assert.IsFalse(cmp2.Equals(mime1, mime2));
    }

    [TestMethod()]
    public void MimeTypeEqualityComparerTest2()
    {
        MimeTypeEqualityComparer c1 = MimeTypeEqualityComparer.Default;
        MimeTypeEqualityComparer c2 = MimeTypeEqualityComparer.Default;

        Assert.AreSame(c1, c2);
    }

    [TestMethod()]
    public void MimeTypeEqualityComparerTest3()
    {
        MimeTypeEqualityComparer c1 = MimeTypeEqualityComparer.IgnoreParameters;
        MimeTypeEqualityComparer c2 = MimeTypeEqualityComparer.IgnoreParameters;

        Assert.AreSame(c1, c2);
    }
}