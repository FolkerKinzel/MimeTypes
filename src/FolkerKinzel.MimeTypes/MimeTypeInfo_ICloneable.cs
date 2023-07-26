namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo : ICloneable
{
    /// <inheritdoc/>
    /// <remarks>
    /// If you intend to hold a <see cref="MimeTypeInfo"/> for a long time in memory and if this <see cref="MimeTypeInfo"/> is parsed
    /// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
    /// keep in mind, that the <see cref="MimeTypeInfo"/> holds a reference to that <see cref="string"/>. Consider in this case to make
    /// a copy of the <see cref="MimeTypeInfo"/> structure: The copy is built on a separate <see cref="string"/>,
    /// which is case-normalized and only as long as needed.
    /// <note type="tip">
    /// Use the instance method <see cref="MimeTypeInfo.Clone"/> if you can, to avoid the costs of boxing.
    /// </note>
    /// </remarks>
    object ICloneable.Clone() => Clone();


    /// <summary>
    /// Creates a new <see cref="MimeTypeInfo"/> that is a copy of the current instance.
    /// </summary>
    /// <returns>A new <see cref="MimeTypeInfo"/>, which is a copy of this instance.</returns>
    /// <remarks>
    /// The copy is built on a separate <see cref="string"/>,
    /// which is case-normalized and only as long as needed.
    /// </remarks>
    public MimeTypeInfo Clone()
    {
        if (this.IsEmpty)
        {
            return default;
        }

        ReadOnlyMemory<char> memory = this._mimeTypeString.ToString().AsMemory();
        return new MimeTypeInfo(in memory, this._idx);
    }

}
