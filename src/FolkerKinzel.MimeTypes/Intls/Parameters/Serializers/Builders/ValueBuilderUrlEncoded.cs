using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderUrlEncoded
{
    private const int KEY_VALUE_SEPARATOR_LENGTH = KeyBuilder.KEY_VALUE_SEPARATOR_LENGTH;
    private const int STAR_LENGTH = 1;
    private const int SINGLE_QUOTES_LENGTH = 2;
    private const string UTF_8 = ParameterSerializer.UTF_8;

    internal static StringBuilder BuildUrlEncoded(this StringBuilder builder,
                                              ReadOnlySpan<char> keySpan,
                                              ReadOnlySpan<char> languageSpan,
                                              string value)
    {
        if (!UrlEncoding.TryEncode(value, out string? encoded))
        {
            return builder;
        }
        value = encoded;

        _ = builder.EnsureCapacity(
            builder.Length + keySpan.Length + STAR_LENGTH + KEY_VALUE_SEPARATOR_LENGTH + UTF_8.Length + languageSpan.Length + SINGLE_QUOTES_LENGTH + value.Length);

        return builder.BuildKey(keySpan) // adds '='
                      .Remove(builder.Length - 1, 1) // removes '='
                      .AppendValueUrlEncoded(languageSpan, value); // adds "*="
    }



    private static StringBuilder AppendValueUrlEncoded(this StringBuilder builder,
                                                       ReadOnlySpan<char> languageSpan,
                                                       string value)
    {
        return builder.Append('*').Append('=').Append(UTF_8).Append('\'').Append(languageSpan).Append('\'').AppendValueUnQuoted(value.AsSpan(), true);
    }
}
