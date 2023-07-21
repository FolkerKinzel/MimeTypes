using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderQuoted
{
    private const int KEY_VALUE_SEPARATOR_LENGTH = KeyBuilder.KEY_VALUE_SEPARATOR_LENGTH;

    internal static StringBuilder BuildQuoted(this StringBuilder builder,
                                             ReadOnlySpan<char> keySpan,
                                             ReadOnlySpan<char> valueSpan,
                                             bool masked,
                                             bool caseSensitive)
    {
        PrepareBuilder(builder, keySpan.Length, valueSpan.Length, masked);

        _ = builder.BuildKey(keySpan); // adds '='
        return masked ? builder.AppendValueQuotedAndMasked(valueSpan, caseSensitive)
                      : builder.AppendValueQuoted(valueSpan, caseSensitive);

        ///////////////////////////////////////////////////////////////////////////////////////

        static void PrepareBuilder(StringBuilder builder, int keyLength, int valueLength, bool masked)
        {
            int neededCapacity = 2 + (masked ? valueLength + 2 : valueLength) + keyLength + KEY_VALUE_SEPARATOR_LENGTH;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);
        }
    }

    private static StringBuilder AppendValueQuoted(this StringBuilder builder, ReadOnlySpan<char> valueSpan, bool isValueCaseSensitive) =>
        builder.Append('\"').AppendValueUnQuoted(valueSpan, isValueCaseSensitive).Append('\"');


    private static StringBuilder AppendValueQuotedAndMasked(this StringBuilder builder,
                                                     ReadOnlySpan<char> value,
                                                     bool isValueCaseSensitive)
    {
        _ = builder.Append('\"');

        int valueStart = builder.Length;
        _ = isValueCaseSensitive
                ? builder.AppendMasked(value)
                : builder.AppendMasked(value).ToLowerInvariant(valueStart);

        return builder.Append('\"');
    }
}
