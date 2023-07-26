using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderQuoted
{
    private const int KEY_VALUE_SEPARATOR_LENGTH = KeyBuilder.KEY_VALUE_SEPARATOR_LENGTH;

    internal static StringBuilder BuildQuoted(this StringBuilder builder,
                                             MimeTypeParameter parameter,
                                             bool masked,
                                             bool caseSensitive)
    {
        PrepareBuilder(builder, parameter, masked);

        _ = builder.BuildKey(parameter.Key); // adds '='
        return masked ? builder.AppendValueQuotedAndMasked(parameter.Value, caseSensitive)
                      : builder.AppendValueQuoted(parameter.Value, caseSensitive);

        ///////////////////////////////////////////////////////////////////////////////////////

        static void PrepareBuilder(StringBuilder builder, MimeTypeParameter parameter, bool masked)
        {
            int valueLength = parameter.Value?.Length ?? 0;
            int neededCapacity = 2 + (masked ? valueLength + 2 : valueLength) + parameter.Key.Length + KEY_VALUE_SEPARATOR_LENGTH;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);
        }
    }

    private static StringBuilder AppendValueQuoted(this StringBuilder builder, string? value, bool isValueCaseSensitive) =>
        builder.Append('\"').AppendValueUnQuoted(value, isValueCaseSensitive).Append('\"');


    private static StringBuilder AppendValueQuotedAndMasked(this StringBuilder builder,
                                                     string? value,
                                                     bool isValueCaseSensitive)
    {
        _ = builder.Append('\"');

        int valueStart = builder.Length;
        _ = isValueCaseSensitive
                ? builder.AppendMasked(value.AsSpan())
                : builder.AppendMasked(value.AsSpan()).ToLowerInvariant(valueStart);

        return builder.Append('\"');
    }
}
