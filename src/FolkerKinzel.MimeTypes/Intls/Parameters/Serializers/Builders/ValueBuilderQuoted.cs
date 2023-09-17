using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderQuoted
{
    private const int KEY_VALUE_SEPARATOR_LENGTH = KeyBuilder.KEY_VALUE_SEPARATOR_LENGTH;

    //internal static StringBuilder BuildQuoted(this StringBuilder builder,
    //                                         MimeTypeParameter parameter,
    //                                         bool masked,
    //                                         bool caseSensitive)
    //    => BuildQuoted(builder, parameter.Key, parameter.Value, masked, caseSensitive);

    internal static StringBuilder BuildQuoted(this StringBuilder builder,
                                         ReadOnlySpan<char> key,
                                         ReadOnlySpan<char> value,
                                         bool masked,
                                         bool caseSensitive)
    {
        PrepareBuilder(builder, key.Length, value.Length, masked);

        _ = builder.BuildKey(key); // adds '='
        return masked ? builder.AppendValueQuotedAndMasked(value, caseSensitive)
                      : builder.AppendValueQuoted(value, caseSensitive);

        ///////////////////////////////////////////////////////////////////////////////////////

        static void PrepareBuilder(StringBuilder builder, int keyLength, int valueLength, bool masked)
        {
            //int valueLength = parameter.Value?.Length ?? 0;
            int neededCapacity = 2 + (masked ? valueLength + 2 : valueLength) + keyLength + KEY_VALUE_SEPARATOR_LENGTH;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);
        }
    }

    private static StringBuilder AppendValueQuoted(this StringBuilder builder, ReadOnlySpan<char> value, bool isValueCaseSensitive) =>
        builder.Append('\"').AppendValueUnQuoted(value, isValueCaseSensitive).Append('\"');


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
