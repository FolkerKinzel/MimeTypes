using FolkerKinzel.Strings.Polyfills;
using System.Runtime.InteropServices;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class ParameterParser
{
    internal static IEnumerable<MimeTypeParameter> ParseParameters(ReadOnlyMemory<char> remainingParameters)
    {
        string currentKey = "";

        StringBuilder? sb = null;
        MimeTypeParameter currentParameter;

        while (!remainingParameters.IsEmpty)
        {
            GetNextParameterMemory(ref remainingParameters, out ReadOnlyMemory<char> nextParameter);

            if (MimeTypeParameter.TryParse(true, ref nextParameter, out MimeTypeParameter parameter))
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
                }
                else // got a splitted parameter
                {
                    if (TryParseParameter(sb, out currentParameter))
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
        int nextParameterSeparatorIndex = ParameterReader.GetNextParameterSeparatorIndex(remainingParameters.Span);

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
