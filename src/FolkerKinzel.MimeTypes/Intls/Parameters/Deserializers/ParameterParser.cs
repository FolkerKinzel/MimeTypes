﻿using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;
using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers;

internal static class ParameterParser
{
    internal static IEnumerable<MimeTypeParameter> ParseParameters(ReadOnlyMemory<char> remainingParameters)
    {
        string currentKey = "";
        bool currentUrlEncoded = false;

        StringBuilder? sb = null;
        MimeTypeParameter currentParameter;

        while (!remainingParameters.IsEmpty)
        {
            GetNextParameterMemory(ref remainingParameters, out ReadOnlyMemory<char> nextParameter);

            if (MimeTypeParameter.TryParse(true, ref nextParameter, out MimeTypeParameter parameter))
            {
                ReadOnlySpan<char> keySpan = parameter.Key;

                // keySpan might have the format "key*1" if the parameter is
                // splitted (see RFC 2231). A trailing '*', which is an indicator that
                // language and/or charset information is present, has yet been eaten by
                // MimeTypeParameter.TryParse
                int splitIndicatorIndex = keySpan.GetSplitIndicatorIndex();
                if (splitIndicatorIndex != -1) // splitted
                {
                    sb ??= new StringBuilder(MimeTypeParameter.STRING_LENGTH);

                    bool isParameterUrlEncoded = keySpan[keySpan.Length - 1] == '*';
                    keySpan = keySpan.Slice(0, splitIndicatorIndex + 1); // key*

                    if (!currentKey.AsSpan().Equals(keySpan, StringComparison.OrdinalIgnoreCase)) // next parameter
                    {
                        currentKey = keySpan.ToString();

                        if (TryParseParameter(sb, out currentParameter)) // return previous splitted parameter that might be in the StringBuilder
                        {
                            yield return currentParameter;
                        }

                        _ = sb.Append(currentKey).Append('=').Append(parameter.CharSet).Append('\'').Append(parameter.Language).Append('\'');
                    }

                    if(currentUrlEncoded && !isParameterUrlEncoded)
                    {
                        // decode the UrlEncoded value in sb
                        // remove the * from the keyName in sb
                        sb.Remove(0, currentKey.Length + 1);
                        if(sb.Contains('%'))
                        {
                            string charset = sb.ToString(0, sb.IndexOf('\''));
                            if(UrlEncoding.TryDecode(sb.ToString(), ))
                        }
                    }

                    // concat with the previous:
                    _ = sb.Append(parameter.Value);
                }
                else // not splitted; NOTE: This MUST be a different parameter than the previous because "key*1" and "key" are different keys.
                {
                    if (TryParseParameter(sb, out currentParameter)) // return previous splitted parameter that might be in the StringBuilder
                    {
                        yield return currentParameter;
                    }

                    currentKey = string.Empty;
                    yield return parameter;
                }
            }
        }//whille


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
