namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameterInfo
{
    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeParameterInfo"/> values are equal.
    /// </summary>
    /// <param name="parameter1">The first <see cref="MimeTypeParameterInfo"/> to compare.</param>
    /// <param name="parameter2">The second <see cref="MimeTypeParameterInfo"/> to compare.</param>
    /// <returns><c>true</c> if <paramref name="parameter1"/> and <paramref name="parameter2"/> are equal;
    /// otherwise, <c>false</c>.</returns>
    public static bool operator ==(MimeTypeParameterInfo parameter1, MimeTypeParameterInfo parameter2)
        => parameter1.Equals(in parameter2);


    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeParameterInfo"/> values are not equal.
    /// </summary>
    /// <param name="parameter1">The first <see cref="MimeTypeParameterInfo"/> to compare.</param>
    /// <param name="parameter2">The second <see cref="MimeTypeParameterInfo"/> to compare.</param>
    /// <returns><c>true</c> if <paramref name="parameter1"/> and <paramref name="parameter2"/> are not equal;
    /// otherwise, <c>false</c>.</returns>
    /// <returns></returns>
    public static bool operator !=(MimeTypeParameterInfo parameter1, MimeTypeParameterInfo parameter2)
        => !parameter1.Equals(in parameter2);

}
