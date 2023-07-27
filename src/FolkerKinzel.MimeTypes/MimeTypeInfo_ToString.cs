using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{
    /// <summary>
    /// Serializes the instance into a <see cref="string"/>.
    /// </summary>
    /// <returns>The string section from which the structure takes the data. 
    /// This may differ from the one used to parse the instance and is not guaranteed 
    /// to conform to Internet Media Type standards. Therefore, only use the 
    /// return value of this method for debugging!</returns>
    public override string ToString() => this._mimeTypeString.ToString();


    /// <summary>
    /// Appends a <see cref="string"/> representation of this instance according to RFC 2045 and RFC 2231 to a 
    /// <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
    public StringBuilder AppendTo(StringBuilder builder) =>
        builder is null ? throw new ArgumentNullException(nameof(builder)) 
                        : builder.Append(this._mimeTypeString);
}
