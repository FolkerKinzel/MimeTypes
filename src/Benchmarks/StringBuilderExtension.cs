using FolkerKinzel.Strings;
using System;
using System.Diagnostics;
using System.Text;

namespace Benchmarks;

public static class StringBuilderExtension
{
    public static StringBuilder AppendUrlEncoded(this StringBuilder sb, ReadOnlySpan<char> value)
    {
        if (sb is null)
        {
            throw new ArgumentNullException(nameof(sb));
        }

        _ = sb.EnsureCapacity((int)(sb.Length + value.Length * 2.3));

        if (value.IsAscii())
        {
            for (int i = 0; i < value.Length; i++)
            {
                sb.AppendCharacter(value[i]);
            }
        }
        else
        {
            byte[] encoded = Encoding.UTF8.GetBytes(value);

            for (int i = 0; i < encoded.Length; i++)
            {
                sb.AppendCharacter((char)encoded[i]);
            }
        }

        return sb;
    }

    public static StringBuilder AppendUrlEncoded2(this StringBuilder sb, ReadOnlySpan<char> value)
    {
        const int shortArray = 256;

        if (sb is null)
        {
            throw new ArgumentNullException(nameof(sb));
        }

        _ = sb.EnsureCapacity((int)(sb.Length + value.Length * 2.3));

        int length = Encoding.UTF8.GetByteCount(value);

        Span<byte> encoded = length > shortArray ? new byte[length] : stackalloc byte[length];
        Encoding.UTF8.GetBytes(value, encoded);

        for (int i = 0; i < encoded.Length; i++)
        {
            sb.AppendCharacter((char)encoded[i]);
        }

        return sb;
    }


    private static void AppendCharacter(this StringBuilder sb, char c)
    {
        if (MustEncode(c))
        {
            sb.AppendHexEncoded(c);
        }
        else
        {
            _ = sb.Append(c);
        }
    }

    private static void AppendHexEncoded(this StringBuilder sb, char c)
        => _ = sb.Append('%').Append(ToHexDigit(c >> 4)).Append(ToHexDigit(c & 0x0F));

    private static char ToHexDigit(int i) =>
        (char)(i < 10 ? i + '0' : i + 'A' - 10);

    private static bool MustEncode(char c) =>
         !(c.IsAsciiLetterOrDigit() || c is '.' or '-' or '_' or '~');


    public static void WriteEncodes()
    {
        for (int i = 0; i < 128; i++)
        {
            string s = Uri.EscapeDataString(new string((char)i, 1));
            Debug.WriteLine($"{i:000} - {(char)i} - {s}");
        }
    }
}
