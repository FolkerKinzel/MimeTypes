using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameterInfo
{

    /// <summary>
    /// Creates a <see cref="string"/> representation of the instance.
    /// </summary>
    /// <returns>A <see cref="string"/> representation of the instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => ToString(false);


    /// <summary>
    /// Creates a <see cref="string"/> representation of the instance.
    /// </summary>
    /// <param name="urlFormat">Pass <c>true</c> to get an URL-encoded string representation 
    /// that can be used within a URI, <c>false</c> to get the default format.</param>
    /// <returns>A <see cref="string"/> representation of the instance.</returns>
    public string ToString(bool urlFormat)
    {
        if (IsEmpty)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        _ = ParameterSerializer.AppendTo(sb, this, urlFormat);
        return sb.ToString();
    }


    /// <summary>
    /// Appends a <see cref="string"/> representation of this instance according to RFC 2045 and RFC 2231
    /// to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/>.</param>
    /// <param name="urlFormat">Pass <c>true</c> to get an URL-encoded string representation 
    /// that can be used within a URI, <c>false</c> to get the default format.</param>
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
    public StringBuilder AppendTo(StringBuilder builder, bool urlFormat = false)
    {
        _ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        if (IsEmpty)
        {
            return builder;
        }

        _ = ParameterSerializer.AppendTo(builder, this, urlFormat);
        return builder;
    }
}
