using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

namespace Benchmarks;


internal static class UrlEncoding
{
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

    [ExcludeFromCodeCoverage]
    internal static bool TryDecode(string value, ReadOnlySpan<char> charSet, [NotNullWhen(true)] out string? decoded)
    {
        try
        {
            decoded = UnescapeValueFromUrlEncoding(value, charSet.IsEmpty || charSet.Equals("utf-8", StringComparison.OrdinalIgnoreCase) ? null : charSet.ToString());
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

        byte[] bytes = ascii.GetBytes(value);

        result = encoding.GetString(WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length));
        return result;
    }

    internal static string UrlEncodePercentSignWithCharset(string? charSet)
    {
        Encoding encoding = TextEncodingConverter.GetEncoding(charSet);
        var bytes = encoding.GetBytes("%");

        return StaticStringMethod.Create(bytes.Length * 3, bytes,
            static (chars, bts) =>
            {
                int idx = 0;
                for (int i = 0; i < bts.Length; i++)
                {
                    byte b = bts[i];
                    chars[idx++] = '%';
                    chars[idx++] = ToHexDigit(b >> 4);
                    chars[idx++] = ToHexDigit(b & 0x0F);
                }
            });


        //StringBuilder sb = new StringBuilder(3);

        //for (int i = 0; i < bytes.Length; i++)
        //{
        //    sb.Append('%');
        //    sb.Append(bytes[i].ToString("X2"));
        //}
        //return sb.ToString();
    }

    private static char ToHexDigit(int i) =>
        (char)(i < 10 ? i + '0' : i + 'a' - 10);
}
