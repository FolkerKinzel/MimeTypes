using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;
using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers;

internal static class ParameterParser
{
    private const int SPLIT_INDEX_INITIAL_VALUE = -1;

    internal static IEnumerable<MimeTypeParameterInfo> ParseParameters(ReadOnlyMemory<char> remainingParameters)
    {
        string currentKey = "";
        int previousSplitIndex = SPLIT_INDEX_INITIAL_VALUE;
        bool splittedParameterStartedUrlEncoded = false;

        StringBuilder? sb = null;
        MimeTypeParameterInfo concatenated;


        while (!remainingParameters.IsEmpty)
        {
            GetNextParameterMemory(ref remainingParameters, out ReadOnlyMemory<char> nextParameter);

            if (!MimeTypeParameterInfo.TryParse(true,
                                                ref nextParameter,
                                                out MimeTypeParameterInfo parameter,
                                                out bool currentParameterStarred))
            {
                yield break;
            }

            ReadOnlySpan<char> keySpan = parameter.Key;

            // keySpan might have the format "key*1" if the parameter is
            // splitted (see RFC 2231). A trailing '*', which is an indicator that
            // language and/or charset information is present, has yet been eaten by
            // MimeTypeParameter.TryParse
            int splitIndicatorIndex = keySpan.GetSplitIndicatorIndex();

            if (splitIndicatorIndex != -1) // splitted
            {
                sb ??= new StringBuilder(parameter.Key.Length +
                                         parameter.Value.Length +
                                         parameter.Language.Length +
                                         parameter.CharSet.Length +
                                         10);

                int startOfSplitIndex = splitIndicatorIndex + 1;
                keySpan = keySpan.Slice(0, startOfSplitIndex); // key*

                if (!currentKey.AsSpan().Equals(keySpan, StringComparison.OrdinalIgnoreCase)) // next parameter
                {
                    previousSplitIndex = SPLIT_INDEX_INITIAL_VALUE;
                    splittedParameterStartedUrlEncoded = currentParameterStarred;
                    currentKey = keySpan.ToString();

                    if (sb.Length != 0)
                    {
                        if (TryParseParameter(sb, out concatenated) && concatenated.Key.IsToken(true))
                        {
                            yield return concatenated;
                        }
                        else
                        {
                            yield break;
                        }
                    }

                    _ = currentParameterStarred
                        ? sb.Append(currentKey).Append('=').Append(parameter.CharSet).Append('\'').Append(parameter.Language).Append('\'')
                        : sb.Append(currentKey.AsSpan(0, currentKey.Length - 1)).Append('=');
                }

                if (!_Int.TryParse(parameter.Key.Slice(startOfSplitIndex), out int currentSplitIndex) ||
                    ++previousSplitIndex != currentSplitIndex)
                {
                    yield break;
                }

                // concat with the previous:
                _ = sb.Append(parameter.Value);

                if (splittedParameterStartedUrlEncoded && !currentParameterStarred)
                {
                    // Quoted parts of a splitted parameter that starts URL encoded 
                    // could contain URL escape sequences like "%C7" or "+". That's why the 
                    // '%' chars and the '+' chars have to be UrlEncoded before the value is appended.
                    UrlEncodeUrlEscapeSigns(currentKey.Length, sb, parameter.Value);
                }
            }
            else // not splitted; NOTE: This MUST be a different parameter than the previous because "key*1" and "key" are different keys.
            {
                if (!StringBuilderIsNullOrEmpty(sb))
                {
                    // return previous splitted parameter that might be in the StringBuilder
                    if (TryParseParameter(sb, out concatenated) && concatenated.Key.IsToken(true))
                    {
                        yield return concatenated;
                    }
                    else
                    {
                        yield break;
                    }
                }

                currentKey = string.Empty;

                if (!parameter.Key.IsToken(true))
                {
                    yield break;
                }

                yield return parameter;
            }
        }//while


        // The last parameter, which might be in the StringBuilder:
        if (!StringBuilderIsNullOrEmpty(sb) &&
            TryParseParameter(sb, out concatenated) &&
            concatenated.Key.IsToken(true))
        {
            yield return concatenated;
        }
    }

    private static void UrlEncodeUrlEscapeSigns(int currentKeyLength, StringBuilder sb, ReadOnlySpan<char> currentValue)
    {
        int currentValueStartIndex = sb.Length - currentValue.Length;

        string? charSet = null;
        if (currentValue.Contains('%'))
        {
            charSet = GetCharSet(sb, currentKeyLength);
            sb.ReplacePercentSignsUrlEncoded(charSet, currentValueStartIndex);
        }

        if (currentValue.Contains('+'))
        {
            charSet ??= GetCharSet(sb, currentKeyLength);
            sb.ReplacePlusSignsUrlEncoded(charSet, currentValueStartIndex);
        }

        static string GetCharSet(StringBuilder sb, int currentKeyLength)
        {
            int lengthOfKeyAndEqualsSign = currentKeyLength + 1;
            string charSet = sb.ToString(lengthOfKeyAndEqualsSign, sb.IndexOf('\'') - lengthOfKeyAndEqualsSign);
            return charSet;
        }
    }

    private static void ReplacePercentSignsUrlEncoded(this StringBuilder sb, string charSet, int currentValueStart)
    {
        Debug.Assert(sb != null);
        Debug.Assert(sb.IndexOf('\'') != -1);

        string replacement =
            charSet.Length == 0 ||
            charSet.Equals("utf-8", StringComparison.OrdinalIgnoreCase) ||
            charSet.Contains("ascii", StringComparison.OrdinalIgnoreCase)
            ? "%25"
            : UrlEncodingHelper.UrlEncodeWithCharset(charSet, "%");

        sb.Replace("%", replacement, currentValueStart);
    }


    private static void ReplacePlusSignsUrlEncoded(this StringBuilder sb, string charSet, int currentValueStart)
    {
        Debug.Assert(sb != null);
        Debug.Assert(sb.IndexOf('\'') != -1);

        string replacement =
            charSet.Length == 0 ||
            charSet.Equals("utf-8", StringComparison.OrdinalIgnoreCase) ||
            charSet.Contains("ascii", StringComparison.OrdinalIgnoreCase)
            ? "%2B"
            : UrlEncodingHelper.UrlEncodeWithCharset(charSet, "+");

        sb.Replace("+", replacement, currentValueStart);
    }



    /// <summary>
    /// Tries to parse the content in a <see cref="StringBuilder"/> as <see cref="MimeTypeParameterInfo"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/>.</param>
    /// <param name="concatenated">The parsed <see cref="MimeTypeParameterInfo"/> concatenated.</param>
    /// <returns><c>false</c> if <paramref name="sb"/> doesn't contain valid data, <c>true</c> otherwise.</returns>
    private static bool TryParseParameter(StringBuilder sb, out MimeTypeParameterInfo concatenated)
    {
        ReadOnlyMemory<char> mem = sb.ToString().AsMemory();
        _ = sb.Clear();

        return MimeTypeParameterInfo.TryParse(false, ref mem, out concatenated, out _);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool StringBuilderIsNullOrEmpty([NotNullWhen(false)] StringBuilder? sb)
        => sb is null || sb.Length == 0;

    private static void GetNextParameterMemory(ref ReadOnlyMemory<char> remainingParameters, out ReadOnlyMemory<char> nextParameter)
    {
        int nextParameterSeparatorIndex = ParameterRawReader.GetNextParameterSeparatorIndex(remainingParameters.Span);

        if (nextParameterSeparatorIndex < 0) // last parameter
        {
            nextParameter = remainingParameters;
            remainingParameters = ReadOnlyMemory<char>.Empty;
        }
        else
        {
            nextParameter = remainingParameters.Slice(0, nextParameterSeparatorIndex);
            remainingParameters = remainingParameters.Slice(nextParameterSeparatorIndex + 1);
        }
    }
}
