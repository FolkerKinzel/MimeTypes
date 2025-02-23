namespace FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

/// <summary>
/// Encodes and decodes quoted HTTP quoted strings
/// </summary>
/// <remarks>
/// <para>The algorithm is described at </para>
/// <list type="bullet">
/// <item>https://mimesniff.spec.whatwg.org/#parsing-a-mime-type</item>
/// <item>https://fetch.spec.whatwg.org/#collect-an-http-quoted-string</item>
/// <item>https://mimesniff.spec.whatwg.org/#serializing-a-mime-type</item>
/// </list>
/// </remarks>
internal static class HttpQuotedString
{
    internal static StringBuilder AppendMasked(this StringBuilder sb,
                                               ReadOnlySpan<char> value)
    {
        sb.EnsureCapacity(sb.Length + (int)(value.Length * 1.5));

        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (c is '\"' or '\\')
            {
                _ = sb.Append('\\');
            }
            sb.Append(c);
        }

        return sb;
    }

    internal static void UnMask(this StringBuilder builder, int startOfValue)
    {
        // See https://fetch.spec.whatwg.org/#collect-an-http-quoted-string
        // If the last character is a Backslash, it remains in the string:
        // "\ => \
        for (int i = startOfValue; i < builder.Length - 1; i++)
        {
            if (builder[i] == '\\')
            {
                // after the mask char one entry can be skipped:
                _ = builder.Remove(i, 1);
            }
        }
    }
}
