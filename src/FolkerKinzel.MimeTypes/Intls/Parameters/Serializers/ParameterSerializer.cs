using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;
using System;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

/// <summary>
/// Serializes <see cref="MimeTypeParameter"/> and <see cref="MimeTypeParameterInfo"/> 
/// objects as character sequence an appends this to a <see cref="StringBuilder"/>.
/// </summary>
internal static class ParameterSerializer
{
    internal const string UTF_8 = "utf-8";

    /// <summary>
    /// Appends a RFC 2231 serialized <see cref="MimeTypeParameter"/>
    /// to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameter"></param>
    /// <param name="urlFormat"></param>
    internal static EncodingAction Append(this StringBuilder builder, MimeTypeParameter parameter, bool urlFormat)
    {
        var value = parameter.Value.AsSpan();
        var language = parameter.Language.AsSpan();

        PrepareBuilder(builder, parameter.Key.Length, value.Length, language.Length);

        return AppendValueTo(builder.Append(parameter.Key),
                             value,
                             language,
                             urlFormat,
                             true);
    }


    ///// <summary>
    ///// Appends a RFC 2231 serialized <see cref="MimeTypeParameterInfo"/>
    ///// to a <see cref="StringBuilder"/>.
    ///// </summary>
    ///// <param name="builder"></param>
    ///// <param name="parameter"></param>
    ///// <param name="urlFormat"></param>
    //internal static EncodingAction Append(this StringBuilder builder, MimeTypeParameterInfo parameter, bool urlFormat)
    //{
    //    var key = parameter.Key;
    //    var value = parameter.Value;
    //    var language = parameter.Language;

    //    PrepareBuilder(builder, key.Length, value.Length, language.Length);

    //    int keyStart = builder.Length;

    //    return AppendValueTo(builder.Append(parameter.Key).ToLowerInvariant(keyStart),
    //                         parameter.Value,
    //                         parameter.Language,
    //                         urlFormat,
    //                         parameter.IsValueCaseSensitive);
    //}


    private static EncodingAction AppendValueTo(StringBuilder builder,
                                         ReadOnlySpan<char> value,
                                         ReadOnlySpan<char> language,
                                         bool urlFormat,
                                         bool caseSensitive)
    {
        Debug.Assert(builder is not null);

        EncodingAction action;

        if (!language.IsEmpty)
        {
            action = EncodingAction.UrlEncode;
        }
        else
        {
            action = value.EncodingAction();
            action = !urlFormat ? action : action.HasFlag(EncodingAction.Quote)
                                                    ? EncodingAction.UrlEncode
                                                    : action;
        }

        builder.Append('=');

        _ = action switch
        {
            EncodingAction.Mask => builder.BuildQuoted(value, true, caseSensitive),
            EncodingAction.Quote => builder.BuildQuoted(value, false, caseSensitive),
            EncodingAction.UrlEncode => builder.BuildUrlEncoded(value, language),
            _ => builder.BuildUnQuoted(value, caseSensitive)
        };

        return action;
    }

    static void PrepareBuilder(StringBuilder builder, int keyLength, int valueLength, int languageLength)
        => _ = builder.EnsureCapacity(builder.Length +
                                      10 +
                                      keyLength +
                                      languageLength +
                                      valueLength);

}
