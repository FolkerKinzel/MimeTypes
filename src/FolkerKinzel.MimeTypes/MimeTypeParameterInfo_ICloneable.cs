namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameterInfo : ICloneable
{
    /// <inheritdoc/>
    /// <remarks>
    /// If you intend to hold a <see cref="MimeTypeParameterInfo"/> for a long time 
    /// in memory and if this <see cref="MimeTypeParameterInfo"/> is parsed from a 
    /// <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from 
    /// a very long <see cref="string"/>, keep in mind, that the 
    /// <see cref="MimeTypeParameterInfo"/> holds a reference to that <see cref="string"/>.
    /// Consider in this case to make a copy of the <see cref="MimeTypeParameterInfo"/> 
    /// structure: The copy is built on a separate <see cref="string"/> that is case-normalized 
    /// and only as long as needed.
    /// <note type="tip">
    /// Use the instance method <see cref="MimeTypeParameterInfo.Clone"/>, if you can, to avoid 
    /// the costs of boxing.
    /// </note>
    /// </remarks>
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Creates a new <see cref="MimeTypeParameterInfo"/> that is a copy of the current instance.
    /// </summary>
    /// <returns>A new <see cref="MimeTypeParameterInfo"/>, which is a copy of this instance.</returns>
    /// <remarks>
    /// The copy is built on a separate <see cref="string"/>,
    /// which is case-normalized and only as long as needed.
    /// </remarks>
    public MimeTypeParameterInfo Clone()
        => IsEmpty ? default
                   : new MimeTypeParameterInfo(_parameterString.ToString().AsMemory(), _idx);
}
