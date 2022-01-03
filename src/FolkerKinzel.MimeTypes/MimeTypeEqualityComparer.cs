namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Implements <see cref="IEqualityComparer{T}"/> to compare <see cref="MimeType"/> instances
/// for equality.
/// </summary>
public class MimeTypeEqualityComparer : IEqualityComparer<MimeType>
{
    private readonly bool _ignoreParameters;

    /// <summary>
    /// Initializes a new <see cref="MimeTypeEqualityComparer"/> object and allows to specify
    /// whether or not the <see cref="MimeType.Parameters"/> shall be ignored in the comparison.
    /// </summary>
    /// <param name="ignoreParameters">Pass in <c>true</c> to let the <see cref="MimeType.Parameters"/> be
    /// ignored in the comparison. The default value is <c>false</c>.</param>
    public MimeTypeEqualityComparer(bool ignoreParameters = false) => this._ignoreParameters = ignoreParameters;

    /// <summary>
    /// Compares two <see cref="MimeType"/> instances for equality.
    /// </summary>
    /// <param name="x">The first <see cref="MimeType"/> instance.</param>
    /// <param name="y">The second <see cref="MimeType"/> instance.</param>
    /// <returns><c>true</c> if <paramref name="x"/> and <paramref name="y"/> are
    /// equal, otherwise <c>false</c>.</returns>
    public bool Equals(MimeType x, MimeType y) => x.Equals(y, _ignoreParameters);

    /// <summary>
    /// Generates a hash code for the <see cref="MimeType"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="MimeType"/> instance to generate a hash code for.</param>
    /// <returns>The hash code.</returns>
    public int GetHashCode([DisallowNull] MimeType obj) => obj.GetHashCode(_ignoreParameters);
}
