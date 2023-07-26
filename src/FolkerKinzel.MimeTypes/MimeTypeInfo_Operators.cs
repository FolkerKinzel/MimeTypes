namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{
    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeInfo"/> instances are equal.
    /// The <see cref="Parameters"/> are taken into account.
    /// </summary>
    /// <param name="mimeType1">The first <see cref="MimeTypeInfo"/> to compare.</param>
    /// <param name="mimeType2">The second <see cref="MimeTypeInfo"/> to compare.</param>
    /// <returns><c>true</c> if <paramref name="mimeType1"/> and <paramref name="mimeType2"/> are equal;
    /// otherwise, <c>false</c>.</returns>
    /// <example>
    /// <para>
    /// Comparing <see cref="MimeTypeInfo"/> instances for equality:
    /// </para>
    /// <code source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample.cs"/>
    /// </example>
    public static bool operator ==(MimeTypeInfo mimeType1, MimeTypeInfo mimeType2) => mimeType1.Equals(in mimeType2, false);


    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeInfo"/> instances are not equal.
    /// The <see cref="Parameters"/> are taken into account.
    /// </summary>
    /// <param name="mimeType1">The first <see cref="MimeTypeInfo"/> to compare.</param>
    /// <param name="mimeType2">The second <see cref="MimeTypeInfo"/> to compare.</param>
    /// <returns><c>true</c> if <paramref name="mimeType1"/> and <paramref name="mimeType2"/> are not equal;
    /// otherwise, <c>false</c>.</returns>
    /// <example>
    /// <para>
    /// Comparing <see cref="MimeTypeInfo"/> instances for equality:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample.cs"/>
    /// </example>
    public static bool operator !=(MimeTypeInfo mimeType1, MimeTypeInfo mimeType2) => !mimeType1.Equals(in mimeType2, false);

}
