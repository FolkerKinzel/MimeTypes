using FolkerKinzel.MimeTypes.Intls.Comparisons;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Represents a <see cref="MimeTypeInfo"/> comparison operation that uses specific comparison rules.
/// </summary>
/// <threadsafety static="true" instance="true"/>
public abstract class MimeTypeInfoEqualityComparer : IEqualityComparer<MimeTypeInfo>
{
    /// <summary>
    /// Compares two <see cref="MimeTypeInfo"/> instances for equality.
    /// </summary>
    /// <param name="x">The first <see cref="MimeTypeInfo"/> instance.</param>
    /// <param name="y">The second <see cref="MimeTypeInfo"/> instance.</param>
    /// <returns><c>true</c> if <paramref name="x"/> and <paramref name="y"/> are
    /// equal, otherwise <c>false</c>.</returns>
    public abstract bool Equals(MimeTypeInfo x, MimeTypeInfo y);


    /// <summary>
    /// Generates a hash code for the <see cref="MimeTypeInfo"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="MimeTypeInfo"/> instance to generate a hash code for.</param>
    /// <returns>The hash code.</returns>
    public abstract int GetHashCode([DisallowNull] MimeTypeInfo obj);


    /// <summary>
    /// Gets the <see cref="MimeTypeInfoEqualityComparer"/> object (Singleton) that performs a default comparison of <see cref="MimeTypeInfo"/> structs
    /// which takes the <see cref="MimeTypeInfo.Parameters"/> into account.
    /// </summary>
    public static MimeTypeInfoEqualityComparer Default { get; } = new MimeTypeEqualityComparerDefault();


    /// <summary>
    /// Gets the <see cref="MimeTypeInfoEqualityComparer"/> object (Singleton) that performs a comparison of <see cref="MimeTypeInfo"/> structs
    /// which ignores the <see cref="MimeTypeInfo.Parameters"/>.
    /// </summary>
    public static MimeTypeInfoEqualityComparer IgnoreParameters { get; } = new MimeTypeEqualityComparerIgnoreParameters();
}
