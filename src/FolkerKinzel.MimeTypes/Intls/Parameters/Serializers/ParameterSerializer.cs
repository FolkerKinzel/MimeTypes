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
    internal const int STRING_LENGTH = 32;

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


    /// <summary>
    /// Appends a RFC 2231 serialized <see cref="MimeTypeParameterInfo"/>
    /// to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameter"></param>
    /// <param name="urlFormat"></param>
    internal static EncodingAction Append(this StringBuilder builder, MimeTypeParameterInfo parameter, bool urlFormat)
    {
        var key = parameter.Key;
        var value = parameter.Value;
        var language = parameter.Language;

        PrepareBuilder(builder, key.Length, value.Length, language.Length);

        int keyStart = builder.Length;

        // keys are case-insensitive (RFC 2231/7.)
        return AppendValueTo(builder.Append(parameter.Key).ToLowerInvariant(keyStart),
                             parameter.Value,
                             parameter.Language,
                             urlFormat,
                             parameter.IsValueCaseSensitive);
    }


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


    internal static int SplitParameter(StringBuilder builder,
                                       StringBuilder worker,
                                       int maxLineLength,
                                       int keyLength,
                                       int languageLength,
                                       bool appendSpace,
                                       EncodingAction action,
                                       int currentLineLength)
    {
        maxLineLength = ComputeMinimumLineLength(keyLength + languageLength,
                                                 maxLineLength,
                                                 action);

        if (worker.Length > maxLineLength)
        {
            foreach (StringBuilder tmp in
                ParameterSplitter.SplitParameter(worker,
                                                 maxLineLength,
                                                 action))
            {
                _ = builder.Append(';').Append(MimeType.NEW_LINE).Append(tmp);
                currentLineLength = tmp.Length;
            }
        }
        else
        {
            _ = builder.Append(';');
            currentLineLength++;

            int neededLength = worker.Length + currentLineLength;

            if (appendSpace)
            {
                neededLength++;
            }

            if (neededLength > maxLineLength)
            {
                _ = builder.Append(MimeType.NEW_LINE);
                currentLineLength = 0;
            }
            else if (appendSpace)
            {
                _ = builder.Append(' ');
                currentLineLength++;
            }

            _ = builder.Append(worker);
            currentLineLength += worker.Length;
        }
        return currentLineLength;
    }


    /// <summary>
    /// Computes the minimum length that is needed for a line. (Depends on key, charset,
    /// language and the <see cref="EncodingAction"/>.)
    /// </summary>
    /// <param name="givenLength">The length that can't be wrapped (key, language, charset).</param>
    /// <param name="desiredLineLength"></param>
    /// <param name="enc"></param>
    /// <returns>The minimum length that is needed for a line.</returns>
    private static int ComputeMinimumLineLength(int givenLength, int desiredLineLength, EncodingAction enc)
    {
        int minimumLength = givenLength + ParameterSplitter.MINIMUM_VARIABLE_LINE_LENGTH;

        if (enc == EncodingAction.UrlEncode)
        {
            minimumLength += ParameterSerializer.UTF_8.Length + 3; // *''
        }
        else if (enc.HasFlag(EncodingAction.Quote))
        {
            minimumLength += 2; // ""
        }

        if (desiredLineLength < minimumLength)
        {
            desiredLineLength = minimumLength;
        }

        return desiredLineLength;
    }

}
