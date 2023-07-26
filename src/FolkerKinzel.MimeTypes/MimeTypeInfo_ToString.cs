using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{
    /// <summary>
    /// Serializes the instance into an Internet Media Type <see cref="string"/> ("MIME type") using the 
    /// <see cref="MimeFormats.Default"/> format.
    /// </summary>
    /// <returns>A <see cref="string"/> representation of the instance 
    /// according to RFC 2045 and RFC 2231.</returns>
    /// <example>
    /// <para>Serialize a <see cref="MimeTypeInfo"/> instance into a standards-compliant Internet Media Type <see cref="string"/> using several options:</para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FormattingOptionsExample.cs"/>
    /// </example>
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
