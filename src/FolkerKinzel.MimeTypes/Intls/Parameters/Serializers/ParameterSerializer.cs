using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;
using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;
using System;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

/// <summary>
/// Serializes <see cref="ParameterModel"/> and <see cref="MimeTypeParameter"/> 
/// objects as character sequence an appends this to a <see cref="StringBuilder"/>.
/// </summary>
internal static class ParameterSerializer
{
    internal const string UTF_8 = "utf-8";

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
            EncodingAction.Mask => builder.BuildQuoted(model.Key.AsSpan(), valueSpan, true, MimeTypeParameter.GetIsValueCaseSensitive(model.Key)),
            EncodingAction.Quote => builder.BuildQuoted(model.Key.AsSpan(), valueSpan, false, MimeTypeParameter.GetIsValueCaseSensitive(model.Key)),
            EncodingAction.UrlEncode => builder.BuildUrlEncoded(model.Key.AsSpan(), model.Language.AsSpan(), model.Value ?? ""),
            _ => builder.BuildUnQuoted(model.Key.AsSpan(), valueSpan, MimeTypeParameter.GetIsValueCaseSensitive(model.Key))
        };
    }


    /// <summary>
    /// Appends a serialized <see cref="MimeTypeParameter"/> oject to a <see cref="StringBuilder"/>
    /// and allows to choose whether the value should be URL encoded in any case.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameter"></param>
    /// <param name="urlFormat"></param>
    /// <returns>The <see cref="EncodingAction"/> which has been used to serialize the <paramref name="parameter"/>.</returns>
    internal static EncodingAction Append(this StringBuilder builder, in MimeTypeParameter parameter, bool urlFormat)
    {
        if(parameter.IsEmpty)
        {
            return EncodingAction.None;
        }

        ReadOnlySpan<char> valueSpan = parameter.Value;
        EncodingAction action;

        if (!parameter.Language.IsEmpty)
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
            EncodingAction.Mask => builder.BuildQuoted(parameter.Key, valueSpan, true, parameter.IsValueCaseSensitive),
            EncodingAction.Quote => builder.BuildQuoted(parameter.Key, valueSpan, false, parameter.IsValueCaseSensitive),
            EncodingAction.UrlEncode => builder.BuildUrlEncoded(parameter.Key, parameter.Language, parameter.Value.ToString()),
            _ => builder.BuildUnQuoted(parameter.Key, parameter.Value, parameter.IsValueCaseSensitive)
        };

        return action;
    }

}
