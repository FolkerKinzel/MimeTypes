using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderQuoted
{
    internal static StringBuilder BuildQuoted(this StringBuilder builder,
                                              ReadOnlySpan<char> value,
                                              bool masked,
                                              bool caseSensitive)
    {
        builder.Append('\"');
        _ = masked ? builder.AppendValueMasked(value, caseSensitive)
                   : builder.BuildUnQuoted(value, caseSensitive);
        return builder.Append('\"');
    }

    private static StringBuilder AppendValueMasked(this StringBuilder builder,
                                                     ReadOnlySpan<char> value,
                                                     bool isValueCaseSensitive)
    {
        int valueStart = builder.Length;
        return isValueCaseSensitive
                ? builder.AppendMasked(value)
                : builder.AppendMasked(value).ToLowerInvariant(valueStart);
    }
}
