using FolkerKinzel.Strings;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class ParameterRawReader
{
    const char SEPARATOR = ';';
    internal static int GetNextParameterSeparatorIndex(ReadOnlySpan<char> value)
    {
        bool isInQuotes = false;
        bool attributeValueSeparatorFound = false;

        for (int i = 0; i < value.Length; i++)
        {
            char current = value[i];

            if (current == '\\')
            {
                // last char before the string ends is the masking '\\'
                // this must remein in the string
                // see: https://fetch.spec.whatwg.org/#collect-an-http-quoted-string
                if (IsBackSlashLastChar(value, ref i))
                {
                    return i;
                }

                i++; // Mask char: Skip one!
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

            if (current == SEPARATOR && attributeValueSeparatorFound)
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

    private static bool IsBackSlashLastChar(ReadOnlySpan<char> value, ref int current)
    {
        int next = current + 1;
        if (next < value.Length && 
            value[next] == '"' && 
            TryFindSeparatorSkipWhiteSpace(value, ref next))
        {
            current = next;
            return true;
        }
        return false;

        ///////////////////////////////////////////////////////////////////

        static bool TryFindSeparatorSkipWhiteSpace(ReadOnlySpan<char> value, ref int next)
        {
            for (int i = next + 1; i < value.Length; i++)
            {
                char c = value[i];
                if (c == SEPARATOR)
                {
                    next = i;
                    return true;
                }
                if (!c.IsWhiteSpace())
                {
                    return false;
                }
            }
            return false;
        }
    }
}