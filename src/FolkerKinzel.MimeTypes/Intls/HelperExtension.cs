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
                TSpecialKinds current = span[i].AnalyzeTSpecialKind();

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
        private static TSpecialKinds AnalyzeTSpecialKind(this char c)
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


        /// <summary>
        /// Validates a <see cref="string"/> parameter that represents a token.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="paraName">The parameter's name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a valid token.</exception>
        internal static void ValidateTokenParameter(this string value, string paraName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(paraName);
            }

            ThrowHelper.ThrowOnTokenError(value.AsSpan().ValidateToken(), paraName);
        }



        internal static TokenError ValidateToken(this ReadOnlySpan<char> token)
        {
            if (token.Length == 0)
            {
                return TokenError.EmptyString;
            }

            for (int i = 0; i < token.Length; i++)
            {
                char current = token[i];

                if (char.IsWhiteSpace(current))
                {
                    return TokenError.ContainsWhiteSpace;
                }

                if (char.IsControl(current))
                {
                    return TokenError.ContainsControl;
                }

                if (current.AnalyzeTSpecialKind() > TSpecialKinds.None)
                {
                    return TokenError.ContainsTSpecial;
                }

                if (!current.IsAscii())
                {
                    return TokenError.ContainsNonAscii;
                }

                if (current is '*' or '\'' or '%')
                {
                    return TokenError.ContainsReservedCharacter;
                }
            }

            return TokenError.None;
        }

    }
}
