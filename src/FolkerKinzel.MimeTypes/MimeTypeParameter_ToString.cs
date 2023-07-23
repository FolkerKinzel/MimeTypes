using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameter
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
    /// <param name="alwaysUrlEncoded">Pass <c>true</c> to URL encode the <see cref="Value"/>s if they contain forbidden characters (instead
    /// of wrapping them with double quotes and masking the forbidden characters with '\'). Without this option URL encoding is always used 
    /// when a <see cref="Language"/> is specified or when the <see cref="Value"/> contains Non-ASCII characters.</param>
    /// <returns>A <see cref="string"/> representation of the instance.</returns>
    public string ToString(bool alwaysUrlEncoded)
    {
        var sb = new StringBuilder(STRING_LENGTH);
        sb.Append(in this, alwaysUrlEncoded);
        return sb.ToString();
    }


    /// <summary>
    /// Appends a <see cref="string"/> representation of this instance according to RFC 2045, RFC 2046 and RFC 2231
    /// to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/>.</param>
    /// <param name="alwaysUrlEncoded">Pass <c>true</c> to URL encode the <see cref="Value"/>s if they contain forbidden characters (instead
    /// of wrapping them with double quotes and masking the forbidden characters with '\'). Without this option URL encoding is always used 
    /// when a <see cref="Language"/> is specified or when the <see cref="Value"/> contains Non-ASCII characters.</param>
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
    public StringBuilder AppendTo(StringBuilder builder, bool alwaysUrlEncoded = false)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Append(in this, alwaysUrlEncoded);
        return builder;
    }


}
