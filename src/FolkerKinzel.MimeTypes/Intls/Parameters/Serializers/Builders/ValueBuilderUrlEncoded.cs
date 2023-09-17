using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderUrlEncoded
{
    private const int KEY_VALUE_SEPARATOR_LENGTH = KeyBuilder.KEY_VALUE_SEPARATOR_LENGTH;
    private const int STAR_LENGTH = 1;
    private const int SINGLE_QUOTES_LENGTH = 2;
    private const string UTF_8 = ParameterSerializer.UTF_8;

    internal static StringBuilder BuildUrlEncoded(this StringBuilder builder, MimeTypeParameter parameter)
    {
        string value = parameter.Value ?? "";

        _ = builder.EnsureCapacity(
            builder.Length + parameter.Key.Length + STAR_LENGTH + KEY_VALUE_SEPARATOR_LENGTH + UTF_8.Length + parameter.Language?.Length ?? 0 + SINGLE_QUOTES_LENGTH + (int)(value.Length * 2.5));

        return builder.BuildKey(parameter.Key) // adds '='
                      .Remove(builder.Length - 1, 1) // removes '='
                      .AppendValueAndLanguage(parameter.Language, value); // adds "*="
    }



    private static StringBuilder AppendValueAndLanguage(this StringBuilder builder,
                                                       string? language,
                                                       string value)
    {
        return builder.Append('*').Append('=').Append(UTF_8).Append('\'').Append(language).Append('\'').AppendUrlEncoded(value);
    }
}
