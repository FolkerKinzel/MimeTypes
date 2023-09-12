using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class StringBuilderExtension
{
    internal static StringBuilder Replace( this StringBuilder builder, string oldValue, string? newValue, int startIndex )
        => builder.Replace(oldValue, newValue, startIndex, builder.Length - startIndex);

    public static StringBuilder AppendUrlEncoded(this StringBuilder sb, ReadOnlySpan<char> value)
        => UrlEncoding.AppendUrlEncodedTo(sb, value);

#if NET461 || NETSTANDARD2_0
    public static StringBuilder AppendUrlEncoded(this StringBuilder sb, string? value)
        => UrlEncoding.AppendUrlEncodedTo(sb, value.AsSpan());
#endif

}

