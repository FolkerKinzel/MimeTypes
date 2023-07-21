using FolkerKinzel.MimeTypes.Intls.Comparisons;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Represents a <see cref="MimeType"/> comparison operation that uses specific comparison rules.
/// </summary>
/// <threadsafety static="true" instance="true"/>
public abstract class MimeTypeEqualityComparer : IEqualityComparer<MimeType>
{
    /// <summary>
    /// Compares two <see cref="MimeType"/> instances for equality.
    /// </summary>
    /// <param name="x">The first <see cref="MimeType"/> instance.</param>
    /// <param name="y">The second <see cref="MimeType"/> instance.</param>
    /// <returns><c>true</c> if <paramref name="x"/> and <paramref name="y"/> are
    /// equal, otherwise <c>false</c>.</returns>
    public abstract bool Equals(MimeType x, MimeType y);


    /// <summary>
    /// Generates a hash code for the <see cref="MimeType"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="MimeType"/> instance to generate a hash code for.</param>
    /// <returns>The hash code.</returns>
    public abstract int GetHashCode([DisallowNull] MimeType obj);


    /// <summary>
    /// Gets the <see cref="MimeTypeEqualityComparer"/> object (Singleton) that performs a default comparison of <see cref="MimeType"/> structs
    /// that takes the <see cref="MimeType.Parameters"/> into account.
    /// </summary>
    public static MimeTypeEqualityComparer Default { get; } = new MimeTypeEqualityComparerDefault();


    /// <summary>
    /// Gets the <see cref="MimeTypeEqualityComparer"/> object (Singleton) that performs a comparison of <see cref="MimeType"/> structs
    /// that ignores the <see cref="MimeType.Parameters"/>.
    /// </summary>
    public static MimeTypeEqualityComparer IgnoreParameters { get; } = new MimeTypeEqualityComparerIgnoreParameters();
}
