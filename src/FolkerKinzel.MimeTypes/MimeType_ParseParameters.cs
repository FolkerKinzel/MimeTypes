using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System.Net;
using System.Text;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeType
{
    #region ParseParameters

    private IEnumerable<MimeTypeParameter> ParseParameters()
    {
        if (!HasParameters)
        {
            yield break;
        }

        int parameterStartIndex = MediaTypeLength + SubTypeLength + 2;


        string? charset = null;
        string? currentKey = null;
        bool skip = false;

        StringBuilder? sb = null;
        MimeTypeParameter concatenated;

        do
        {
            if (TryParseParameter(ref parameterStartIndex, out MimeTypeParameter parameter, out bool quoted))
            {
                ReadOnlySpan<char> keySpan = parameter.Key;

                // keySpan might have the format "key*1" if the parameter is
                // splitted (see RFC 2184). A trailing '*', which is an indicator that
                // language and/or charset information is present, has yet been eaten by
                // MimeTypeParameter.TryParse
                if (keySpan.Length >= 2 && keySpan[keySpan.Length - 2] == '*')
                {
                    sb ??= new StringBuilder(MimeTypeParameter.STRING_LENGTH);

                    keySpan = keySpan.Slice(0, keySpan.Length - 1);

                    if (!currentKey.AsSpan().Equals(keySpan, StringComparison.OrdinalIgnoreCase))
                    {
                        skip = false;
                        currentKey = keySpan.ToString();
                        charset = parameter.Charset.ToString();

                        if (BuildConcatenated(sb, out concatenated))
                        {
                            yield return concatenated;
                        }

                        ReadOnlySpan<char> languageSpan = parameter.Language;
                        _ = languageSpan.IsEmpty
                            // Remove the trailing '*' from currentKey:
                            ? sb.Append(currentKey).Remove(sb.Length - 1, 1).Append('=')
                            // The charset is not preserved because parameter.Value is passed decoded:
                            : sb.Append(currentKey).Append('=').Append('\'').Append(parameter.Language).Append('\'');
                    }

                    if (skip)
                    {
                        continue;
                    }

                    // concat with the previous:
                    ReadOnlySpan<char> valueSpan = parameter.Value;
                    if (!quoted && valueSpan.Contains('%'))
                    {
                        try
                        {
                            _ = sb.Append(UnescapeValueFromUrlEncoding(charset, valueSpan.ToString()));
                        }
                        catch
                        {
                            // This makes the parameter unreadable:
                            _ = sb.Clear();
                            skip = true;
                        }
                    }
                    else
                    {
                        _ = sb.Append(valueSpan);
                    }
                    continue;
                }
                else
                {
                    if (BuildConcatenated(sb, out concatenated))
                    {
                        yield return concatenated;
                    }

                    skip = false;
                    currentKey = null;
                    ReadOnlySpan<char> charsetSpan = parameter.Charset;
                    charset = charsetSpan.IsEmpty ? null : charsetSpan.ToString();

                    if (RemoveUrlEncoding(charset, parameter.Value, quoted, ref sb, ref parameter))
                    {
                        yield return parameter;
                    }
                }
            }
        }
        while (parameterStartIndex != -1);

        // The last parameter, which might be in the StringBuilder:
        if (BuildConcatenated(sb, out concatenated))
        {
            yield return concatenated;
        }
    }


    private static bool BuildConcatenated(StringBuilder? sb, out MimeTypeParameter concatenated)
    {
        if (sb is not null && sb.Length != 0)
        {
            ReadOnlyMemory<char> mem = sb.ToString().AsMemory();
            _ = sb.Clear();

            return MimeTypeParameter.TryParse(false, ref mem, out concatenated, out _);
        }

        concatenated = default;
        return false;
    }

    private static bool RemoveUrlEncoding(string? charset, ReadOnlySpan<char> valueSpan, bool quoted, ref StringBuilder? sb, ref MimeTypeParameter parameter)
    {
        if (!quoted && valueSpan.Contains('%'))
        {
            string result;

            try
            {
                result = UnescapeValueFromUrlEncoding(charset, valueSpan.ToString());
            }
            catch
            {
                return false;
            }

            ReadOnlyMemory<char> memory;
            ReadOnlySpan<char> languageSpan = parameter.Language;
            ReadOnlySpan<char> keySpan = parameter.Key;

            // The charset is not needed anymore because result is always passed
            // unescaped and unquoted:
            if (languageSpan.IsEmpty)
            {
                sb ??= new StringBuilder(keySpan.Length + 1 + result.Length);
                memory = sb.Append(keySpan).Append('=').Append(result).ToString().AsMemory();
                _ = sb.Clear();
            }
            else
            {
                sb ??= new StringBuilder(keySpan.Length + 4 + languageSpan.Length + result.Length);

                memory =
                    sb.Append(keySpan).Append('*').Append('=').Append('\'').Append(languageSpan).Append('\'').Append(result)
                    .ToString().AsMemory();
                _ = sb.Clear();
            }

            return MimeTypeParameter.TryParse(false, ref memory, out parameter, out _);
        }

        return true;
    }

    /// <summary>
    /// Removes URL encoding from <paramref name="value"/>.
    /// </summary>
    /// <param name="charset"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="DecoderFallbackException"></exception>
    /// <exception cref="EncoderFallbackException"></exception>
    private static string UnescapeValueFromUrlEncoding(string? charset, string value)
    {
        string result;

        EncoderFallback encoderFallback = EncoderFallback.ExceptionFallback;
        DecoderFallback decoderFallback = DecoderFallback.ExceptionFallback;

        Encoding encoding = TextEncodingConverter.GetEncoding(charset, encoderFallback, decoderFallback);
        Encoding ascii = TextEncodingConverter.GetEncoding(20127, encoderFallback, decoderFallback);

        ascii.EncoderFallback = EncoderFallback.ExceptionFallback;
        byte[] bytes = ascii.GetBytes(value);

        result = encoding.GetString(WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length));
        return result;
    }

    private bool TryParseParameter(ref int parameterStartIndex, out MimeTypeParameter parameter, out bool quoted)
    {
        int nextParameterSeparatorIndex = GetNextParameterSeparatorIndex(_mimeTypeString.Span.Slice(parameterStartIndex));
        ReadOnlyMemory<char> currentParameterString;

        if (nextParameterSeparatorIndex < 0) // last parameter
        {
            currentParameterString = _mimeTypeString.Slice(parameterStartIndex);
            parameterStartIndex = -1;
        }
        else
        {
            currentParameterString = _mimeTypeString.Slice(parameterStartIndex, nextParameterSeparatorIndex);
            parameterStartIndex += nextParameterSeparatorIndex + 1;
        }

        return MimeTypeParameter.TryParse(true, ref currentParameterString, out parameter, out quoted);
    }


    private static int GetNextParameterSeparatorIndex(ReadOnlySpan<char> value)
    {
        bool isInQuotes = false;
        bool attributeValueSeparatorFound = false;

        for (int i = 0; i < value.Length; i++)
        {
            char current = value[i];

            if (current == '\\') // Mask char: Skip one!
            {
                i++;
                continue;
            }

            if (current == '"')
            {
                isInQuotes = !isInQuotes;
            }

            if (isInQuotes)
            {
                continue;
            }

            if (current == ';' && attributeValueSeparatorFound)
            {
                return i;
            }

            if (current == '=')
            {
                if (attributeValueSeparatorFound)
                {
                    return -1;
                }

                attributeValueSeparatorFound = true;
                continue;
            }

            // Don't test control characters because
            // newline characters are control characters
            if (current.IsTSpecial() || !current.IsAscii())
            {
                return -1;
            }

            //if(current == '(') // Start of a comment..
            //{
            //    return -1;
            //}


        }

        return -1;
    }

    #endregion

}
