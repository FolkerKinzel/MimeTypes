namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameterInfo : IComparable<MimeTypeParameterInfo>
{
    /// <summary>
    /// Compares the current instance with another <see cref="MimeTypeParameterInfo"/> and 
    /// returns an <see cref="int"/> that indicates whether the current instance precedes, 
    /// follows, or occurs in the same position in the sort order as the other 
    /// <see cref="MimeTypeParameterInfo"/>.
    /// </summary>
    /// <param name="other">The other <see cref="MimeTypeParameterInfo"/> instance to compare
    /// with.</param>
    /// <returns>A value that indicates the relative order of the instances being compared.
    /// </returns>
    /// <remarks>The method takes only the <see cref="MimeTypeParameterInfo.Key"/>s into 
    /// account.</remarks>
    public int CompareTo(MimeTypeParameterInfo other)
        => Key.CompareTo(other.Key, StringComparison.OrdinalIgnoreCase);
}
