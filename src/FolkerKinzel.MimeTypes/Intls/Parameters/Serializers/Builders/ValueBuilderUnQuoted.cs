namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderUnQuoted
{
    private const int KEY_VALUE_SEPARATOR_LENGTH = KeyBuilder.KEY_VALUE_SEPARATOR_LENGTH;

    internal static StringBuilder BuildUnQuoted(this StringBuilder builder,
                                            ReadOnlySpan<char> key,
                                            ReadOnlySpan<char> value,
                                            bool caseSensitive)
    {
        PrepareBuilder(builder, key.Length, value.Length);

        return builder.BuildKey(key).AppendValueUnQuoted(value, caseSensitive);

        ////////////////////////////////////////////////////////////////

        static void PrepareBuilder(StringBuilder builder, int keyLength, int valueLength)
        {
            int neededCapacity = valueLength + keyLength + KEY_VALUE_SEPARATOR_LENGTH;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);
        }
    }


    internal static StringBuilder AppendValueUnQuoted(this StringBuilder builder, ReadOnlySpan<char> value, bool isValueCaseSensitive)
    {
        int valueStart = builder.Length;
        return isValueCaseSensitive
                ? builder.Append(value)
                : builder.Append(value).ToLowerInvariant(valueStart);
    }
}
