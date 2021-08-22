using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes.Properties;
using FolkerKinzel.Strings;

namespace FolkerKinzel.MimeTypes.Intls
{
    internal static class HelperExtension
    {
        internal static TSpecialKinds AnalyzeTSpecials(this ReadOnlySpan<char> span)
        {
            TSpecialKinds result = TSpecialKinds.None;

            for (int i = 0; i < span.Length; i++)
            {
                TSpecialKinds current = span[i].Analyze();

                if (current == TSpecialKinds.MaskChar)
                {
                    return current;
                }

                if (current > result)
                {
                    result = current;
                }
            }
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0066:Switch-Anweisung in Ausdruck konvertieren", Justification = "<Ausstehend>")]
        internal static TSpecialKinds Analyze(this char c)
        {
            // RFC 2045 Section 5.1 "tspecials"
            switch (c)
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
                    return TSpecialKinds.TSpecial;
                case '\\':
                case '\"':
                    return TSpecialKinds.MaskChar;
                default:
                    return TSpecialKinds.None;
            }
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

        internal static void ValidateKey(this string key, string paraName)
        {
            if (key is null)
            {
                throw new ArgumentNullException(paraName);
            }

            if (key.Length == 0)
            {
                throw new ArgumentException(string.Format(Res.EmptyString, paraName), paraName);
            }

            for (int i = 0; i < key.Length; i++)
            {
                char current = key[i];

                if (char.IsWhiteSpace(current))
                {
                    throw new ArgumentException(string.Format(Res.ContainsWhiteSpace, paraName), paraName);
                }

                if (char.IsControl(current))
                {
                    throw new ArgumentException(string.Format(Res.ContainsControlCharacter, paraName), paraName);
                }

                if (current.Analyze() > TSpecialKinds.None)
                {
                    throw new ArgumentException(string.Format(Res.ContainsTSpecial, paraName), paraName);
                }

                if (!current.IsAscii())
                {
                    throw new ArgumentException(string.Format(Res.ContainsNonAscii, paraName), paraName);
                }

                if (current is '*' or '\'' or '%')
                {
                    throw new ArgumentException(string.Format(Res.ContainsReservedCharacter, paraName), paraName);
                }
            }
        }

    }
}
