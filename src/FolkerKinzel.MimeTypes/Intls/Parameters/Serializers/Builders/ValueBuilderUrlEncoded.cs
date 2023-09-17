using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;


namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class ValueBuilderUrlEncoded
{
    private const int KEY_VALUE_SEPARATOR_LENGTH = KeyBuilder.KEY_VALUE_SEPARATOR_LENGTH;
    private const int STAR_LENGTH = 1;
    private const int SINGLE_QUOTES_LENGTH = 2;
    private const string UTF_8 = ParameterSerializer.UTF_8;

    internal static StringBuilder BuildUrlEncoded(this StringBuilder builder,
                                                 ReadOnlySpan<char> key,
                                                 ReadOnlySpan<char> value,
                                                 ReadOnlySpan<char> language)
    {
        PrepareBuilder(builder, key.Length, value.Length, language.Length);
            
        return builder.BuildKey(key) // adds '='
                      .Remove(builder.Length - 1, 1) // removes '='
                      .AppendValueAndLanguage(language, value); // adds "*="

        /////////////////////////////////////////////////////////////
        
        static void PrepareBuilder(StringBuilder builder, int keyLength, int valueLength, int languageLength)
        => _ = builder.EnsureCapacity(builder.Length + 
                                      keyLength + 
                                      STAR_LENGTH +
                                      KEY_VALUE_SEPARATOR_LENGTH + 
                                      UTF_8.Length + 
                                      languageLength + 
                                      SINGLE_QUOTES_LENGTH + 
                                      (int)(valueLength * 2.5));
    }

    private static StringBuilder AppendValueAndLanguage(this StringBuilder builder,
                                                       ReadOnlySpan<char> language,
                                                       ReadOnlySpan<char> value)
    {
        return builder.Append('*').Append('=').Append(UTF_8).Append('\'').Append(language).Append('\'').AppendUrlEncoded(value);
    }
}
