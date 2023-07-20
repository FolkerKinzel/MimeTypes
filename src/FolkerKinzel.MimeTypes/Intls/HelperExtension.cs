﻿using FolkerKinzel.Strings;
using System.Reflection;

namespace FolkerKinzel.MimeTypes.Intls;


internal static class HelperExtension
{
    internal static int DigitsCount(this int input) =>
        input == 0 ? 1 : (int)Math.Floor(Math.Log10(input) + 1);


    internal static TSpecialKinds AnalyzeTSpecials(this ReadOnlySpan<char> span)
    {
        TSpecialKinds result = TSpecialKinds.None;

        for (int i = 0; i < span.Length; i++)
        {
            char c = span[i];

            Debug.Assert(c.IsAscii());

            if (c.IsWhiteSpace() || c.IsControl() || c.IsReservedCharacter())
            {
                result = TSpecialKinds.TSpecial;
            }

            TSpecialKinds current = c.AnalyzeTSpecialKind();

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
            TokenError error = token[i].AnalyzeTokenChar();

            if (error != TokenError.None)
            {
                return error;
            }
        }

        return TokenError.None;
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0066:Switch-Anweisung in Ausdruck konvertieren", Justification = "<Ausstehend>")]
    private static TSpecialKinds AnalyzeTSpecialKind(this char c)
    {
        // RFC 2045 Section 5.1 "tspecials" ('.' is not tspecial)
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
            if (isTextMimeType && parameter.IsAsciiCharSetParameter)
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


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsTSpecial(this char c) => c.AnalyzeTSpecialKind() != TSpecialKinds.None;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsReservedCharacter(this char c) => c is '*' or '\'' or '%'; // RFC 2184
        


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:In bedingten Ausdruck konvertieren", Justification = "<Ausstehend>")]
    private static TokenError AnalyzeTokenChar(this char c)
    {
        if (char.IsWhiteSpace(c))
        {
            return TokenError.ContainsWhiteSpace;
        }

        if (char.IsControl(c))
        {
            return TokenError.ContainsControl;
        }

        if (c.IsTSpecial())
        {
            return TokenError.ContainsTSpecial;
        }

        if (!c.IsAscii())
        {
            return TokenError.ContainsNonAscii;
        }

        if (c.IsReservedCharacter()) // RFC 2184
        {
            return TokenError.ContainsReservedCharacter;
        }

        return TokenError.None;
    }

}
