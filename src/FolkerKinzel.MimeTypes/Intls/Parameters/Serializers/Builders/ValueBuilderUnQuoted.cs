namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderUnQuoted
{
    private const int KEY_VALUE_SEPARATOR_LENGTH = KeyBuilder.KEY_VALUE_SEPARATOR_LENGTH;

    internal static StringBuilder BuildUnQuoted(this StringBuilder builder, ReadOnlySpan<char> keySpan, ReadOnlySpan<char> valueSpan, bool caseSensitive)
    {
        PrepareBuilder(builder, keySpan.Length, valueSpan.Length);

        return builder.BuildKey(keySpan).AppendValueUnQuoted(valueSpan, caseSensitive);

        ////////////////////////////////////////////////////////////////

        static void PrepareBuilder(StringBuilder builder, int keyLength, int valueLength)
        {
            int neededCapacity = valueLength + keyLength + KEY_VALUE_SEPARATOR_LENGTH;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);
        }
    }


    internal static StringBuilder AppendValueUnQuoted(this StringBuilder builder, ReadOnlySpan<char> valueSpan, bool isValueCaseSensitive)
    {
        int valueStart = builder.Length;
        return isValueCaseSensitive
                ? builder.Append(valueSpan)
                : builder.Append(valueSpan).ToLowerInvariant(valueStart);
    }
}
