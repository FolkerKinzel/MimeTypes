using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using FolkerKinzel.Strings;

namespace Benchmarks;

public static class StringBuilderExtension
{
    public static StringBuilder AppendUrlEncoded(this StringBuilder sb, ReadOnlySpan<char> value)
    {
        if (sb is null)
        {
            throw new ArgumentNullException(nameof(sb));
        }

        _ = sb.EnsureCapacity((int)(sb.Length + value.Length * 1.5));

        if (value.IsAscii())
        {
            for (int i = 0; i < value.Length; i++)
            {
                sb.AppendCharacter(value[i]);
            }
        }
        else
        {
            var encoded = Encoding.UTF8.GetBytes(value);

            for (int i = 0; i < encoded.Length; i++)
            {
                sb.AppendCharacter((char)encoded[i]);
            }
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
    {
        _ = sb.Append('%').Append(ToHexDigit(c >> 4)).Append(ToHexDigit(c & 0x0F));

        static char ToHexDigit(int i) =>
        (char)(i < 10 ? i + '0' : i + 'a' - 10);
    }


    private static bool MustEncode(char c) =>
         !(c.IsAsciiLetter() || c.IsDecimalDigit() || c is '.' or '-' or '_' or '~');


    public static void WriteEncodes()
    {
        for (int i = 0; i < 128; i++)
        {
            string s = Uri.EscapeDataString(new string((char)i, 1));
            Debug.WriteLine($"{i:000} - {(char)i} - {s}");
        }
    }
}
