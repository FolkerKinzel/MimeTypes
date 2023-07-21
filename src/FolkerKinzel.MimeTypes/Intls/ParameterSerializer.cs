using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Encodings;
using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
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

        bool starred = model.Language != null || action == EncodingAction.UrlEncode;

        if (starred && model.Value != null)
        {
            if (!UrlEncoding.TryEncode(model.Value!, out string? encoded))
            {
                return;
            }
            valueSpan = encoded.AsSpan();
        }

        _ = builder.Append(model.Key);

        _ = action switch
        {
            EncodingAction.Mask => builder.Append('=').AppendValueQuotedAndMasked(valueSpan, true),
            EncodingAction.Quote => builder.Append('=').AppendValueQuoted(valueSpan, true),

            // This adds '=':
            _ => AppendValueUrlEncoded(builder,
                                       action != EncodingAction.UrlEncode,
                                       valueSpan,
                                       model.Language.AsSpan(),
                                       starred),
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

        bool urlEncoded = alwaysUrlEncoded || !parameter.Language.IsEmpty || action == EncodingAction.UrlEncode;

        if (urlEncoded)
        {
            return builder.BuildUrlEncoded(in parameter, action != EncodingAction.UrlEncode);
        }

        return action == EncodingAction.None
                ? builder.BuildUnQuoted(in parameter)
                : builder.BuildQuoted(in parameter, action == EncodingAction.Mask);
    }


    private static StringBuilder BuildUrlEncoded(this StringBuilder builder, in MimeTypeParameter parameter, bool isValueAscii)
    {
        ReadOnlySpan<char> keySpan = parameter.Key;
        ReadOnlySpan<char> valueSpan = parameter.Value;
        ReadOnlySpan<char> languageSpan = parameter.Language;

        if (!UrlEncoding.TryEncode(valueSpan.ToString(), out string? encoded))
        {
            return builder;
        }
        valueSpan = encoded.AsSpan();

        bool starred = !isValueAscii || !languageSpan.IsEmpty;
        _ = builder.EnsureCapacity(builder.Length + GetNeededCapacity(keySpan.Length, valueSpan.Length, languageSpan.Length, isValueAscii, starred));
        return builder.BuildKey(keySpan) // adds '='
                      .Remove(builder.Length - 1, 1) // removes '='
                      .AppendValueUrlEncoded(isValueAscii, valueSpan, languageSpan, starred); // adds '='

        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        static int GetNeededCapacity(int keySpanLength, int valueSpanLength, int languageSpanLength, bool isValueAscii, bool starred)
        {

            int charsetLength = isValueAscii ? 0 : UTF_8.Length;

            //                                                                =
            int neededCapacity = valueSpanLength + keySpanLength + KEY_VALUE_SEPARATOR_LENGTH;
            if (starred)
            {
                //                *                  ' '
                neededCapacity += 1 + charsetLength + 2 + languageSpanLength;
            }
            return neededCapacity;
        }
    }


    private static StringBuilder BuildQuoted(this StringBuilder builder, in MimeTypeParameter parameter, bool masked)
    {
        ReadOnlySpan<char> valueSpan = parameter.Value;
        ReadOnlySpan<char> keySpan = parameter.Key;

        PrepareBuilder(builder, keySpan.Length, valueSpan.Length, masked);

        _ = builder.BuildKey(keySpan); // adds '='
        return masked ? builder.AppendValueQuotedAndMasked(valueSpan, parameter.IsValueCaseSensitive)
                      : builder.AppendValueQuoted(valueSpan, parameter.IsValueCaseSensitive);

        ///////////////////////////////////////////////////////////////////////////////////////

        static void PrepareBuilder(StringBuilder builder, int keyLength, int valueLength, bool masked)
        {
            int neededCapacity = 2 + (masked ? valueLength + 2 : valueLength) + keyLength + KEY_VALUE_SEPARATOR_LENGTH;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);
        }
    }


    private static StringBuilder BuildUnQuoted(this StringBuilder builder, in MimeTypeParameter parameter)
    {
        ReadOnlySpan<char> valueSpan = parameter.Value;
        ReadOnlySpan<char> keySpan = parameter.Key;

        PrepareBuilder(builder, keySpan.Length, valueSpan.Length);

        return builder.BuildKey(keySpan).AppendValueUnQuoted(valueSpan, parameter.IsValueCaseSensitive);

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
                                                     bool isValueAscii,
                                                     ReadOnlySpan<char> valueSpan,
                                                     ReadOnlySpan<char> languageSpan,
                                                     bool starred)
    {
        return starred
            ? builder.Append('*').Append('=').Append(isValueAscii ? "" : UTF_8).Append('\'').Append(languageSpan).Append('\'').AppendValueUnQuoted(valueSpan, true)
            : builder.Append('=').AppendValueUnQuoted(valueSpan, true);
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
