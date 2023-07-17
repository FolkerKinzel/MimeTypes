﻿using FolkerKinzel.Strings;
using System.Net;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls;


internal static class UrlEncoding
{
    [ExcludeFromCodeCoverage]
    internal static bool TryEncode(string input, [NotNullWhen(true)] out string? output)
    {
        Debug.Assert(input != null);
        try
        {
            output = Uri.EscapeDataString(input);
        }
        catch
        {
            output = null;
            return false;
        }
        return true;
    }

    internal static bool TryDecode(string value, ReadOnlySpan<char> charSet, [NotNullWhen(true)] out string? decoded)
    {
        try
        {
            decoded = UnescapeValueFromUrlEncoding(value, charSet.IsEmpty ? null : charSet.ToString());
            return true;
        }
        catch
        {
            decoded = null;
            return false;
        }
    }

    /// <summary>
    /// Removes URL encoding from <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="charSet"></param>
    /// <returns></returns>
    /// <exception cref="DecoderFallbackException"></exception>
    /// <exception cref="EncoderFallbackException"></exception>
    private static string UnescapeValueFromUrlEncoding(string value, string? charSet)
    {
        string result;

        EncoderFallback encoderFallback = EncoderFallback.ExceptionFallback;
        DecoderFallback decoderFallback = DecoderFallback.ExceptionFallback;

        Encoding encoding = TextEncodingConverter.GetEncoding(charSet, encoderFallback, decoderFallback);
        Encoding ascii = TextEncodingConverter.GetEncoding(20127, encoderFallback, decoderFallback);

        ascii.EncoderFallback = EncoderFallback.ExceptionFallback;
        byte[] bytes = ascii.GetBytes(value);

        result = encoding.GetString(WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length));
        return result;
    }
}