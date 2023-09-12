using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using FolkerKinzel.Strings;
using Microsoft.Diagnostics.Tracing.Stacks.Formats;

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


    internal static bool TryDecode(ReadOnlySpan<char> value, string? charSet, [NotNullWhen(true)] out string? decoded)
    {
        try
        {
            decoded = UnescapeValueFromUrlEncoding(value, string.IsNullOrEmpty(charSet) ? "utf-8" : charSet);
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
    private static string UnescapeValueFromUrlEncoding(ReadOnlySpan<char> value, string charSet)
    {
        const int shortString = 256;
        const byte spaceChar = (byte)' ';

        Encoding encoding = TextEncodingConverter.GetEncoding(charSet,
                                                              EncoderFallback.ExceptionFallback,
                                                              DecoderFallback.ExceptionFallback,
                                                              true);

        Span<byte> bytes = value.Length > shortString ? new byte[value.Length]
                                                      : stackalloc byte[value.Length];

        int byteIndex = 0;

        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];

            if (c > 127)
            {
                throw new EncoderFallbackException();
            }

            switch(c)
            {
                case '+':
                    bytes[byteIndex++] = spaceChar;
                    continue;
                case '%':
                    bytes[byteIndex++] = byte.Parse(value.Slice(i + 1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    i += 2;
                    continue;
                default:
                    bytes[byteIndex++] = (byte)c;
                    continue;
            }
        }

        return encoding.GetString(bytes.Slice(0, byteIndex));
    }


    

}
