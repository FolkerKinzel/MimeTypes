﻿using FolkerKinzel.MimeTypes.Properties;
using FolkerKinzel.Strings;
using System.Reflection;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class HelperExtension
{
    internal static int DigitsCount(this int input) =>
        input == 0 ? 1 : (int)Math.Floor(Math.Log10(input) + 1);


    //internal static TokenError ValidateToken(this ReadOnlySpan<char> token)
    //{
    //    if (token.Length == 0)
    //    {
    //        return TokenError.EmptyString;
    //    }

    //    for (int i = 0; i < token.Length; i++)
    //    {
    //        TokenError error = token[i].AnalyzeTokenChar();

    //        if (error != TokenError.None)
    //        {
    //            return error;
    //        }
    //    }

    //    return TokenError.None;
    //}


    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0066:Switch-Anweisung in Ausdruck konvertieren", Justification = "<Ausstehend>")]
    //private static EncodingAction AnalyzeTSpecialKind(this char c)
    //{
    //    // RFC 2045 Section 5.1 "tspecials" ('.' is not tspecial)
    //    switch (c)
    //    {
    //        case '(':
    //        case ')':
    //        case '<':
    //        case '>':
    //        case '@':
    //        case ',':
    //        case ';':
    //        case ':':
    //        case '/':
    //        case '[':
    //        case ']':
    //        case '?':
    //        case '=':
    //            return EncodingAction.Quote; 
                
    //                                            // There is nothing said in the RFCs about masking of '\"', but:
    //        case '\\':                          // https://mimesniff.spec.whatwg.org/#parsing-a-mime-type  § 4.5.:
    //        case '\"':                          // 4.1. Precede each occurrence of U+0022 (") or U+005C (\) in value with U+005C (\).
    //            return EncodingAction.Mask;  // 4.2. Prepend U+0022 (") to value. // 4.3. Append U+0022 (") to value.
    //        default:
    //            return EncodingAction.None;
    //    }
    //}




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


    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static bool IsTSpecial(this char c) => c.AnalyzeTSpecialKind() != TSpecialKinds.None;


    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static bool IsReservedCharacter(this char c) => c is '*' or '\'' or '%'; // RFC 2231
        


    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:In bedingten Ausdruck konvertieren", Justification = "<Ausstehend>")]
    //private static TokenError AnalyzeTokenChar(this char c)
    //{
    //    if (char.IsWhiteSpace(c))
    //    {
    //        return TokenError.ContainsWhiteSpace;
    //    }

    //    if (char.IsControl(c))
    //    {
    //        return TokenError.ContainsControl;
    //    }

    //    if (c.IsTSpecial())
    //    {
    //        return TokenError.ContainsTSpecial;
    //    }

    //    if (!c.IsAscii())
    //    {
    //        return TokenError.ContainsNonAscii;
    //    }

    //    if (c.IsReservedCharacter()) // RFC 2231
    //    {
    //        return TokenError.ContainsReservedCharacter;
    //    }

    //    return TokenError.None;
    //}

}
