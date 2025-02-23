namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameterInfo
{
    /// <summary>
    /// Returns a value that indicates whether two specified 
    /// <see cref="MimeTypeParameterInfo"/> values are equal.
    /// </summary>
    /// <param name="parameterInfo1">The first <see cref="MimeTypeParameterInfo"/>
    /// to compare.</param>
    /// <param name="parameterInfo2">The second <see cref="MimeTypeParameterInfo"/> 
    /// to compare.</param>
    /// <returns><c>true</c> if <paramref name="parameterInfo1"/> and 
    /// <paramref name="parameterInfo2"/> are equal; otherwise, <c>false</c>.</returns>
    /// <seealso cref="Equals(in MimeTypeParameterInfo)"/>
    public static bool operator ==(MimeTypeParameterInfo parameterInfo1,
                                   MimeTypeParameterInfo parameterInfo2)
        => parameterInfo1.Equals(in parameterInfo2);

    /// <summary>
    /// Returns a value that indicates whether two specified 
    /// <see cref="MimeTypeParameterInfo"/> values are not equal.
    /// </summary>
    /// <param name="parameterInfo1">The first <see cref="MimeTypeParameterInfo"/>
    /// to compare.</param>
    /// <param name="parameterInfo2">The second <see cref="MimeTypeParameterInfo"/>
    /// to compare.</param>
    /// <returns><c>true</c> if <paramref name="parameterInfo1"/> and 
    /// <paramref name="parameterInfo2"/> are not equal; otherwise, <c>false</c>.</returns>
    /// <seealso cref="Equals(in MimeTypeParameterInfo)"/>
    public static bool operator !=(MimeTypeParameterInfo parameterInfo1,
                                   MimeTypeParameterInfo parameterInfo2)
        => !parameterInfo1.Equals(in parameterInfo2);
}
