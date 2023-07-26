namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderUnQuoted
{
    private const int KEY_VALUE_SEPARATOR_LENGTH = KeyBuilder.KEY_VALUE_SEPARATOR_LENGTH;

    internal static StringBuilder BuildUnQuoted(this StringBuilder builder, MimeTypeParameter parameter, bool caseSensitive)
    {
        PrepareBuilder(builder, parameter);

        return builder.BuildKey(parameter.Key).AppendValueUnQuoted(parameter.Value, caseSensitive);

        ////////////////////////////////////////////////////////////////

        static void PrepareBuilder(StringBuilder builder, MimeTypeParameter parameter)
        {
            int neededCapacity = parameter.Value?.Length ?? 0 + parameter.Key.Length + KEY_VALUE_SEPARATOR_LENGTH;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);
        }
    }


    internal static StringBuilder AppendValueUnQuoted(this StringBuilder builder, string? value, bool isValueCaseSensitive)
    {
        int valueStart = builder.Length;
        return isValueCaseSensitive
                ? builder.Append(value)
                : builder.Append(value).ToLowerInvariant(valueStart);
    }
}
