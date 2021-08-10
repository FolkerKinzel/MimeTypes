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


        
        internal static IEnumerable<MimeTypeParameter> Sort(this IEnumerable<MimeTypeParameter> parameters, bool isTextMimeType)
        {
            List<MimeTypeParameter>? list = null;

            foreach (MimeTypeParameter parameter in parameters)
            {
                if (isTextMimeType && parameter.IsAsciiCharsetParameter)
                {
                    continue;
                }

                list ??= new List<MimeTypeParameter>(2);
                list.Add(parameter);
            }

            if (list is null)
            {
                return Array.Empty<MimeTypeParameter>();
            }

            if (list.Count == 1)
            {
                return list;
            }

            list.Sort();

            return list;
        }


    }
}
