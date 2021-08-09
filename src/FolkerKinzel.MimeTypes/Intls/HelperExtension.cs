using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Strings;

namespace FolkerKinzel.MimeTypes.Intls
{
    internal static class HelperExtension
    {
        internal static bool ContainsTSpecials(this ReadOnlySpan<char> span, out bool containsMaskChars)
        {
            containsMaskChars = false;

            // RFC 2045 Section 5.1 "tspecials"
            for (int i = 0; i < span.Length; i++)
            {
                switch (span[i])
                {
                    case '(':
                    case ')':
                    case '<':
                    case '>':
                    case '@':
                    case ',':
                    case ';':
                    case ':':

                    case '/':
                    case '[':
                    case ']':
                    case '?':
                    case '=':
                        return true;
                    case '\\':
                    case '\"':
                        containsMaskChars = true;
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }

    }
}
