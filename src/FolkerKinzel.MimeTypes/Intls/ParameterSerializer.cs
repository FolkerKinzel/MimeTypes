using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Encodings;
using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System.Reflection;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls;

/// <summary>
/// Serializes <see cref="ParameterModel"/> and <see cref="MimeTypeParameter"/> 
/// objects as character sequence an appends this to a <see cref="StringBuilder"/>.
/// </summary>
internal static class ParameterSerializer
{
    internal const string UTF_8 = "utf-8";
    private const int KEY_VALUE_SEPARATOR_LENGTH = 1;

    /// <summary>
    /// Appends a RFC 2231 serialized <see cref="ParameterModel"/>
    /// to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="model"></param>
    internal static void Append(StringBuilder builder, in ParameterModel model)
    {
        Debug.Assert(builder is not null);

        ReadOnlySpan<char> valueSpan = model.Value.AsSpan();
        EncodingAction action = valueSpan.EncodingAction();
        action = model.Language != null ? EncodingAction.UrlEncode : action;

        _ = action switch
        {
            EncodingAction.Mask => builder.BuildQuoted(model.Key.AsSpan(), valueSpan, true, true),
            EncodingAction.Quote => builder.BuildQuoted(model.Key.AsSpan(), valueSpan, false, true),
            EncodingAction.UrlEncode => builder.BuildUrlEncoded(model.Key.AsSpan(), model.Language.AsSpan(), model.Value ?? ""),
            _ => builder.BuildUnQuoted(model.Key.AsSpan(), model.Value.AsSpan(), true)
        };
    }


    /// <summary>
    /// Appends a serialized <see cref="MimeTypeParameter"/> oject to a <see cref="StringBuilder"/>
    /// and allows to choose whether the value should be URL encoded in any case.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameter"></param>
    /// <param name="alwaysUrlEncoded"></param>
    /// <returns><paramref name="builder"/></returns>
    internal static StringBuilder Append(this StringBuilder builder, in MimeTypeParameter parameter, bool alwaysUrlEncoded)
    {
        ReadOnlySpan<char> valueSpan = parameter.Value;
        EncodingAction action = valueSpan.EncodingAction();

        action = alwaysUrlEncoded || !parameter.Language.IsEmpty ? EncodingAction.UrlEncode : action;

        return action switch
        {
            EncodingAction.Mask => builder.BuildQuoted(parameter.Key, valueSpan, true, parameter.IsValueCaseSensitive),
            EncodingAction.Quote => builder.BuildQuoted(parameter.Key, valueSpan, false, parameter.IsValueCaseSensitive),
            EncodingAction.UrlEncode => builder.BuildUrlEncoded(parameter.Key, parameter.Language, parameter.Value.ToString()),
            _ => builder.BuildUnQuoted(parameter.Key, parameter.Value, parameter.IsValueCaseSensitive)
        };
    }


    private static StringBuilder BuildUrlEncoded(this StringBuilder builder,
                                                 ReadOnlySpan<char> keySpan,
                                                 ReadOnlySpan<char> languageSpan,
                                                 string value)
    {
        if (!UrlEncoding.TryEncode(value, out string? encoded))
        {
            return builder;
        }
        value = encoded;

        _ = builder.EnsureCapacity(builder.Length + keySpan.Length + 1 + KEY_VALUE_SEPARATOR_LENGTH + UTF_8.Length + languageSpan.Length + 2 + value.Length);
        
        return builder.BuildKey(keySpan) // adds '='
                      .Remove(builder.Length - 1, 1) // removes '='
                      .AppendValueUrlEncoded(languageSpan, value); // adds '='
    }


    private static StringBuilder BuildQuoted(this StringBuilder builder,
                                             ReadOnlySpan<char> keySpan,
                                             ReadOnlySpan<char> valueSpan,
                                             bool masked,
                                             bool caseSensitive)
    {
        PrepareBuilder(builder, keySpan.Length, valueSpan.Length, masked);

        _ = builder.BuildKey(keySpan); // adds '='
        return masked ? builder.AppendValueQuotedAndMasked(valueSpan, caseSensitive)
                      : builder.AppendValueQuoted(valueSpan, caseSensitive);

        ///////////////////////////////////////////////////////////////////////////////////////

        static void PrepareBuilder(StringBuilder builder, int keyLength, int valueLength, bool masked)
        {
            int neededCapacity = 2 + (masked ? valueLength + 2 : valueLength) + keyLength + KEY_VALUE_SEPARATOR_LENGTH;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);
        }
    }


    private static StringBuilder BuildUnQuoted(this StringBuilder builder, ReadOnlySpan<char> keySpan, ReadOnlySpan<char> valueSpan, bool caseSensitive)
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


    private static StringBuilder BuildKey(this StringBuilder builder, ReadOnlySpan<char> keySpan)
    {
        int keyStart = builder.Length;
        return builder.Append(keySpan).ToLowerInvariant(keyStart).Append('=');
    }


    private static StringBuilder AppendValueUrlEncoded(this StringBuilder builder,
                                                       ReadOnlySpan<char> languageSpan,
                                                       string value)
    {
        return builder.Append('*').Append('=').Append(UTF_8).Append('\'').Append(languageSpan).Append('\'').AppendValueUnQuoted(value.AsSpan(), true);
    }


    private static StringBuilder AppendValueQuoted(this StringBuilder builder, ReadOnlySpan<char> valueSpan, bool isValueCaseSensitive) =>
        builder.Append('\"').AppendValueUnQuoted(valueSpan, isValueCaseSensitive).Append('\"');


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


    private static StringBuilder AppendValueUnQuoted(this StringBuilder builder, ReadOnlySpan<char> valueSpan, bool isValueCaseSensitive)
    {
        int valueStart = builder.Length;
        return isValueCaseSensitive
                ? builder.Append(valueSpan)
                : builder.Append(valueSpan).ToLowerInvariant(valueStart);
    }
    
}
