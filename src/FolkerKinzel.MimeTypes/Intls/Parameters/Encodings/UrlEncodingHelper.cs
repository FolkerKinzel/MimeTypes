using System.Net;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

internal static class UrlEncodingHelper
{
    //    public static StringBuilder AppendUrlEncodedTo(StringBuilder sb, ReadOnlySpan<char> value)
    //    {
    //        if (sb is null)
    //        {
    //            throw new ArgumentNullException(nameof(sb));
    //        }

    //        _ = sb.EnsureCapacity((int)(sb.Length + value.Length * 2.3));

    //#if NET461 || NETSTANDARD2_0
    //        byte[] encoded = Encoding.UTF8.GetBytes(value);
    //#else
    //        const int shortArray = 256;
    //        int length = Encoding.UTF8.GetByteCount(value);
    //        Span<byte> encoded = length > shortArray ? new byte[length] : stackalloc byte[length];
    //        Encoding.UTF8.GetBytes(value, encoded);
    //#endif
    //        for (int i = 0; i < encoded.Length; i++)
    //        {
    //            sb.AppendCharacter((char)encoded[i]);
    //        }

    //        return sb;
    //    }


    //    private static void AppendCharacter(this StringBuilder sb, char c)
    //    {
    //        if (MustEncode(c))
    //        {
    //            sb.AppendHexEncoded(c);
    //        }
    //        else
    //        {
    //            _ = sb.Append(c);
    //        }
    //    }

    //    private static void AppendHexEncoded(this StringBuilder sb, char c)
    //        => _ = sb.Append('%').Append(ToHexDigit(c >> 4)).Append(ToHexDigit(c & 0x0F));



    //    private static bool MustEncode(char c) =>
    //         !(c.IsAsciiLetter() || c.IsDecimalDigit() || c is '.' or '-' or '_' or '~');


    //    internal static bool TryDecode(ReadOnlySpan<char> value, string? charSet, [NotNullWhen(true)] out string? decoded)
    //    {
    //        try
    //        {
    //            decoded = UnescapeValueFromUrlEncoding(value, string.IsNullOrEmpty(charSet) ? "utf-8" : charSet);
    //            return true;
    //        }
    //        catch
    //        {
    //            decoded = null;
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// Removes URL encoding from <paramref name="value"/>.
    //    /// </summary>
    //    /// <param name="value"></param>
    //    /// <param name="charSet"></param>
    //    /// <returns></returns>
    //    /// <exception cref="DecoderFallbackException"></exception>
    //    /// <exception cref="EncoderFallbackException"></exception>
    //    private static string UnescapeValueFromUrlEncoding(ReadOnlySpan<char> value, string charSet)
    //    {
    //        const int shortString = 256;
    //        const byte spaceChar = (byte)' ';

    //        Encoding encoding = TextEncodingConverter.GetEncoding(charSet,
    //                                                              EncoderFallback.ExceptionFallback,
    //                                                              DecoderFallback.ExceptionFallback,
    //                                                              true);

    //        Span<byte> bytes = value.Length > shortString ? new byte[value.Length]
    //                                                      : stackalloc byte[value.Length];

    //        int byteIndex = 0;

    //        for (int i = 0; i < value.Length; i++)
    //        {
    //            char c = value[i];

    //            if (c > 127)
    //            {
    //                throw new EncoderFallbackException();
    //            }

    //            switch (c)
    //            {
    //                case '+':
    //                    bytes[byteIndex++] = spaceChar;
    //                    continue;
    //                case '%':
    //#if NET461 || NETSTANDARD2_0
    //                    bytes[byteIndex++] = byte.Parse(value.Slice(i + 1, 2).ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
    //#else
    //                    bytes[byteIndex++] = byte.Parse(value.Slice(i + 1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
    //#endif
    //                    i += 2;
    //                    continue;
    //                default:
    //                    bytes[byteIndex++] = (byte)c;
    //                    continue;
    //            }
    //        }

    //        return encoding.GetString(bytes.Slice(0, byteIndex));
    //    }


    internal static string UrlEncodeWithCharset(string? charSet, string value)
    {
        Encoding encoding = TextEncodingConverter.GetEncoding(charSet);
        var bytes = encoding.GetBytes(value);

        return StaticStringMethod.Create(bytes.Length * 3, bytes, encodeContent);
        //static (chars, bts) =>
        //{
        //    int idx = 0;
        //    for (int i = 0; i < bts.Length; i++)
        //    {
        //        byte b = bts[i];
        //        chars[idx++] = '%';
        //        chars[idx++] = ToHexDigit(b >> 4);
        //        chars[idx++] = ToHexDigit(b & 0x0F);
        //    }


        //});

        static void encodeContent(Span<char> chars, byte[] bts)
        {
            int idx = 0;
            for (int i = 0; i < bts.Length; i++)
            {
                byte b = bts[i];
                chars[idx++] = '%';
                chars[idx++] = ToHexDigit(b >> 4);
                chars[idx++] = ToHexDigit(b & 0x0F);
            }

            static char ToHexDigit(int i) => (char)(i < 10 ? i + '0' : i + 'A' - 10);
        }
    }



}
