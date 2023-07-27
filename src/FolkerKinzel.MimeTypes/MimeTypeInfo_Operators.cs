namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{
    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeInfo"/> instances are equal.
    /// The <see cref="Parameters"/> are taken into account.
    /// </summary>
    /// <param name="info1">The first <see cref="MimeTypeInfo"/> to compare.</param>
    /// <param name="info2">The second <see cref="MimeTypeInfo"/> to compare.</param>
    /// <returns><c>true</c> if <paramref name="info1"/> and <paramref name="info2"/> are equal;
    /// otherwise, <c>false</c>.</returns>
    /// <seealso cref="Equals(in MimeTypeInfo)"/>
    public static bool operator ==(MimeTypeInfo info1, MimeTypeInfo info2) => info1.Equals(in info2, false);


    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeInfo"/> instances are not equal.
    /// The <see cref="Parameters"/> are taken into account.
    /// </summary>
    /// <param name="info1">The first <see cref="MimeTypeInfo"/> to compare.</param>
    /// <param name="info2">The second <see cref="MimeTypeInfo"/> to compare.</param>
    /// <returns><c>true</c> if <paramref name="info1"/> and <paramref name="info2"/> are not equal;
    /// otherwise, <c>false</c>.</returns>
    /// <seealso cref="Equals(in MimeTypeInfo)"/>
    public static bool operator !=(MimeTypeInfo info1, MimeTypeInfo info2) => !info1.Equals(in info2, false);

}
