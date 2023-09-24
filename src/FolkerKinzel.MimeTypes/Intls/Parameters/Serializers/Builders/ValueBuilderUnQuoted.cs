namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderUnQuoted
{
    internal static StringBuilder BuildUnQuoted(this StringBuilder builder,
                                                ReadOnlySpan<char> value,
                                                bool caseSensitive)
    {
        int valueStart = builder.Length;
        return caseSensitive
                ? builder.Append(value)
                : builder.Append(value).ToLowerInvariant(valueStart);
    }
}
