using FolkerKinzel.Strings;
using System.Runtime.InteropServices;
using System.Text;

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