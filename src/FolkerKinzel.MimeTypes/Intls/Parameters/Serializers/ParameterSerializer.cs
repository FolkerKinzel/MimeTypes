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
    internal static EncodingAction Append(this StringBuilder builder, MimeTypeParameter parameter, bool urlFormat)
    {
        Debug.Assert(builder is not null);

        ReadOnlySpan<char> valueSpan = parameter.Value.AsSpan();
        EncodingAction action;

        if (parameter.Language != null)
        {
            action = EncodingAction.UrlEncode;
        }
        else
        {
            action = valueSpan.EncodingAction();
            action = !urlFormat ? action : action.HasFlag(EncodingAction.Quote)
                                                    ? EncodingAction.UrlEncode
                                                    : action;
        }

        _ = action switch
        {
            EncodingAction.Mask => builder.BuildQuoted(parameter, true, MimeTypeParameterInfo.GetIsValueCaseSensitive(parameter.Key)),
            EncodingAction.Quote => builder.BuildQuoted(parameter, false, MimeTypeParameterInfo.GetIsValueCaseSensitive(parameter.Key)),
            EncodingAction.UrlEncode => builder.BuildUrlEncoded(parameter),
            _ => builder.BuildUnQuoted(parameter, MimeTypeParameterInfo.GetIsValueCaseSensitive(parameter.Key))
        };

        return action;
    }


    ///// <summary>
    ///// Appends a serialized <see cref="MimeTypeParameterInfo"/> oject to a <see cref="StringBuilder"/>
    ///// and allows to choose whether the value should be URL encoded in any case.
    ///// </summary>
    ///// <param name="builder"></param>
    ///// <param name="parameter"></param>
    ///// <param name="urlFormat"></param>
    ///// <returns>The <see cref="EncodingAction"/> which has been used to serialize the <paramref name="parameter"/>.</returns>
    //internal static EncodingAction Append(this StringBuilder builder, in MimeTypeParameterInfo parameter, bool urlFormat)
    //{
    //    if(parameter.IsEmpty)
    //    {
    //        return EncodingAction.None;
    //    }

    //    ReadOnlySpan<char> valueSpan = parameter.Value;
    //    EncodingAction action;

    //    if (!parameter.Language.IsEmpty)
    //    {
    //        action = EncodingAction.UrlEncode;
    //    }
    //    else
    //    {
    //        action = valueSpan.EncodingAction();
    //        action = !urlFormat ? action : action.HasFlag(EncodingAction.Quote)
    //                                                ? EncodingAction.UrlEncode
    //                                                : action;
    //    }

    //    _ = action switch
    //    {
    //        EncodingAction.Mask => builder.BuildQuoted(parameter.Key, valueSpan, true, parameter.IsValueCaseSensitive),
    //        EncodingAction.Quote => builder.BuildQuoted(parameter.Key, valueSpan, false, parameter.IsValueCaseSensitive),
    //        EncodingAction.UrlEncode => builder.BuildUrlEncoded(parameter.Key, parameter.Language, parameter.Value.ToString()),
    //        _ => builder.BuildUnQuoted(parameter.Key, parameter.Value, parameter.IsValueCaseSensitive)
    //    };

    //    return action;
    //}

}
