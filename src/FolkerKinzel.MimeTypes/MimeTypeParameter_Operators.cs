namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameter
{
    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeParameter"/> instances are equal.
    /// </summary>
    /// <param name="mimeTypeParameter1">The first <see cref="MimeTypeParameter"/> to compare.</param>
    /// <param name="mimeTypeParameter2">The second <see cref="MimeTypeParameter"/> to compare.</param>
    /// <returns><c>true</c> if <paramref name="mimeTypeParameter1"/> and <paramref name="mimeTypeParameter2"/> are equal;
    /// otherwise, <c>false</c>.</returns>
    public static bool operator ==(MimeTypeParameter mimeTypeParameter1, MimeTypeParameter mimeTypeParameter2)
        => mimeTypeParameter1.Equals(in mimeTypeParameter2);

    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeParameter"/> instances are not equal.
    /// </summary>
    /// <param name="mimeTypeParameter1">The first <see cref="MimeTypeParameter"/> to compare.</param>
    /// <param name="mimeTypeParameter2">The second <see cref="MimeTypeParameter"/> to compare.</param>
    /// <returns><c>true</c> if <paramref name="mimeTypeParameter1"/> and <paramref name="mimeTypeParameter2"/> are not equal;
    /// otherwise, <c>false</c>.</returns>
    /// <returns></returns>
    public static bool operator !=(MimeTypeParameter mimeTypeParameter1, MimeTypeParameter mimeTypeParameter2)
        => !mimeTypeParameter1.Equals(in mimeTypeParameter2);

}
