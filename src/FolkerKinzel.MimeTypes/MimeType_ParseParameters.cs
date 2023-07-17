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

        int nextParameterStartIndex = MediaTypeLength + SubTypeLength + 2;

        string currentKey = "";

        StringBuilder? sb = null;
        MimeTypeParameter currentParameter;

        do
        {
            ReadOnlySpan<char> mimeTypeSpan = _mimeTypeString.Span;
            nextParameterStartIndex = GetNextParameterMemory(nextParameterStartIndex,
                                                             mimeTypeSpan.Slice(nextParameterStartIndex),
                                                             out ReadOnlyMemory<char> nextParameterMemory);


            if (MimeTypeParameter.TryParse(true, ref nextParameterMemory, out MimeTypeParameter parameter))
            {
                ReadOnlySpan<char> keySpan = parameter.Key;

                // keySpan might have the format "key*1" if the parameter is
                // splitted (see RFC 2184). A trailing '*', which is an indicator that
                // language and/or charset information is present, has yet been eaten by
                // MimeTypeParameter.TryParse
                if (keySpan.Length >= 2 && keySpan[keySpan.Length - 2] == '*')
                {
                    sb ??= new StringBuilder(MimeTypeParameter.STRING_LENGTH);

                    keySpan = keySpan.Slice(0, keySpan.Length - 1); // key*

                    if (!currentKey.AsSpan().Equals(keySpan, StringComparison.OrdinalIgnoreCase))
                    {
                        currentKey = keySpan.ToString();

                        if (TryParseParameter(sb, out currentParameter))
                        {
                            yield return currentParameter;
                        }

                        _ = sb.Append(currentKey).Append('=').Append(parameter.Charset).Append('\'').Append(parameter.Language).Append('\'');
                    }

                    // concat with the previous:
                    _ = sb.Append(parameter.Value);
                    continue;
                }
                else // bot a splitted parameter
                {
                    if (TryParseParameter(sb, out currentParameter))
                    {
                        yield return currentParameter;
                    }

                    currentKey = string.Empty;

                    yield return parameter;
                }
            }
        }
        while (nextParameterStartIndex != -1);

        // The last parameter, which might be in the StringBuilder:
        if (TryParseParameter(sb, out currentParameter))
        {
            yield return currentParameter;
        }
    }


    /// <summary>
    /// Tries to parse the content in a <see cref="StringBuilder"/> as <see cref="MimeTypeParameter"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/>.</param>
    /// <param name="concatenated">The parsed <see cref="MimeTypeParameter"/> concatenated.</param>
    /// <returns><c>false</c> if <paramref name="sb"/> is <c>null</c> or empty or if it doesn't contain valid data, <c>true</c> otherwise.</returns>
    private static bool TryParseParameter(StringBuilder? sb, out MimeTypeParameter concatenated)
    {
        if (sb is not null && sb.Length != 0)
        {
            ReadOnlyMemory<char> mem = sb.ToString().AsMemory();
            _ = sb.Clear();

            return MimeTypeParameter.TryParse(false, ref mem, out concatenated);
        }

        concatenated = default;
        return false;
    }



    //private static bool RemoveUrlEncoding(string? charset, ReadOnlySpan<char> valueSpan, bool quoted, ref StringBuilder? sb, ref MimeTypeParameter parameter)
    //{
    //    if (!quoted && valueSpan.Contains('%'))
    //    {

    //        if (!UrlEncoding.TryDecode(valueSpan.ToString(), charset, out string? result))
    //        {
    //            return false;
    //        }


    //        ReadOnlyMemory<char> memory;
    //        ReadOnlySpan<char> languageSpan = parameter.Language;
    //        ReadOnlySpan<char> keySpan = parameter.Key;

    //        // The charset is not needed anymore because result is always passed
    //        // unescaped and unquoted:
    //        if (languageSpan.IsEmpty)
    //        {
    //            sb ??= new StringBuilder(keySpan.Length + 1 + result.Length);
    //            memory = sb.Append(keySpan).Append('=').Append(result).ToString().AsMemory();
    //            _ = sb.Clear();
    //        }
    //        else
    //        {
    //            sb ??= new StringBuilder(keySpan.Length + 4 + languageSpan.Length + result.Length);

    //            memory =
    //                sb.Append(keySpan).Append('*').Append('=').Append('\'').Append(languageSpan).Append('\'').Append(result)
    //                .ToString().AsMemory();
    //            _ = sb.Clear();
    //        }

    //        return MimeTypeParameter.TryParse(false, ref memory, out parameter, out _);
    //    }

    //    return true;
    //}


    //private bool TryParseParameter(ref int parameterStartIndex, out MimeTypeParameter parameter, out bool quoted)
    //{
    //    int nextParameterSeparatorIndex = GetNextParameterSeparatorIndex(_mimeTypeString.Span.Slice(parameterStartIndex));
    //    ReadOnlyMemory<char> nextParameterMemory;

    //    if (nextParameterSeparatorIndex < 0) // last parameter
    //    {
    //        nextParameterMemory = _mimeTypeString.Slice(parameterStartIndex);
    //        parameterStartIndex = -1;
    //    }
    //    else
    //    {
    //        nextParameterMemory = _mimeTypeString.Slice(parameterStartIndex, nextParameterSeparatorIndex);
    //        parameterStartIndex += nextParameterSeparatorIndex + 1;
    //    }

    //    return MimeTypeParameter.TryParse(true, ref nextParameterMemory, out parameter, out quoted);

    //}


    private int GetNextParameterMemory(int nextParameterStartIndex, ReadOnlySpan<char> remainingParameters, out ReadOnlyMemory<char> parameterMemory)
    {
        int nextParameterSeparatorIndex = GetNextParameterSeparatorIndex(remainingParameters);

        if (nextParameterSeparatorIndex < 0) // last parameter
        {
            parameterMemory = _mimeTypeString.Slice(nextParameterStartIndex);
            nextParameterStartIndex = -1;
        }
        else
        {
            parameterMemory = _mimeTypeString.Slice(nextParameterStartIndex, nextParameterSeparatorIndex);
            nextParameterStartIndex += nextParameterSeparatorIndex + 1;
        }

        return nextParameterStartIndex;

        ////////////////////////////////////////////////////////////////////////

        static int GetNextParameterSeparatorIndex(ReadOnlySpan<char> value)
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
    }


    #endregion

}