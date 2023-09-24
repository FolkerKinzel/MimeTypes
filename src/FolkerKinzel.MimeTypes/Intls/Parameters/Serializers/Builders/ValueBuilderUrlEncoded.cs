using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;


namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderUrlEncoded
{
    private const int STAR_LENGTH = 1;
    private const int SINGLE_QUOTES_LENGTH = 2;
    private const string UTF_8 = ParameterSerializer.UTF_8;

    internal static StringBuilder BuildUrlEncoded(this StringBuilder builder,
                                                  ReadOnlySpan<char> value,
                                                  ReadOnlySpan<char> language)
    {
        PrepareBuilder(builder, value.Length, language.Length);
            
        return builder.Insert(builder.Length - 1,'*').AppendValueAndLanguage(language, value);

        /////////////////////////////////////////////////////////////
        
        static void PrepareBuilder(StringBuilder builder, int valueLength, int languageLength)
        => _ = builder.EnsureCapacity(builder.Length + 
                                      STAR_LENGTH +
                                      UTF_8.Length + 
                                      languageLength + 
                                      SINGLE_QUOTES_LENGTH + 
                                      (int)(valueLength * UrlEncoding.EncodedLengthFactor));
    }

    private static StringBuilder AppendValueAndLanguage(this StringBuilder builder,
                                                       ReadOnlySpan<char> language,
                                                       ReadOnlySpan<char> value)
    {
        return builder.Append(UTF_8).Append('\'').Append(language).Append('\'').AppendUrlEncoded(value);
    }
}
