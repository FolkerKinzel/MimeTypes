using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.MimeTypes.Tests;

[TestClass]
public class MimeTypeParameterTests
{
    [TestMethod]
    public void CompareToTest1()
    {
        MimeTypeParameter? other = null;
        MimeTypeParameter test = MimeType.Create("x", "y").AppendParameter("a", "b").Parameters.First();

        Assert.AreEqual(1, test.CompareTo(other));
    }

    [TestMethod]
    public void EqualsTest1()
    {
        MimeTypeParameter? other = null;
        MimeTypeParameter test = MimeType.Create("x", "y").AppendParameter("a", "b").Parameters.First();

        Assert.IsFalse(test.Equals(other));
        Assert.IsFalse(test == other);
        Assert.IsTrue(test != other);

        Assert.IsFalse(test.Equals(42));

        object third = test;
        Assert.IsTrue(test.Equals(third));
    }

    [TestMethod]
    public void EqualsOperatorTest1()
    {
        MimeTypeParameter? m1 = null;
        MimeTypeParameter? m2 = null;
        Assert.IsTrue(m1 == m2);
        Assert.IsFalse(m1 != m2);
    }

    [TestMethod]
    public void EqualsOperatorTest2()
    {
        MimeTypeParameter? m1 = MimeType.Create("x", "y").AppendParameter("a", "b").Parameters.First();
        MimeTypeParameter? m2 = null;
        Assert.IsTrue(m1 != m2);
        Assert.IsTrue(m2 != m1);

        Assert.IsFalse(m1 == m2);
        Assert.IsFalse(m2 == m1);
        m2 = m1;
        Assert.IsTrue(m1 == m2);

    }

}
