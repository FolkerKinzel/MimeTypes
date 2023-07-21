using FolkerKinzel.Strings;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class ParameterRawReader
{
    internal static int GetNextParameterSeparatorIndex(ReadOnlySpan<char> value)
    {
        bool isInQuotes = false;
        bool attributeValueSeparatorFound = false;

        for (int i = 0; i < value.Length; i++)
        {
            char current = value[i];

            if (current == '\\') // Mask char: Skip one!
            {
                int overNext = i + 2;
                if (overNext < value.Length && value[overNext - 1] == '"' && value[overNext] == ';')
                {
                    // last char before the string ends is the masking '\\'
                    // this must remein in the string
                    // see: https://fetch.spec.whatwg.org/#collect-an-http-quoted-string
                    continue;
                }

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

            // Uncomment for a thorough validation:
            //// Don't test control characters because
            //// newline characters are control characters
            //if (current.IsTSpecial() || !current.IsAscii())
            //{
            //    return -1;
            //}

        }

        return -1;
    }
}