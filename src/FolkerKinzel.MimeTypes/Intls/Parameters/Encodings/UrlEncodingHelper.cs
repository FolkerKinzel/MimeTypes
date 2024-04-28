namespace FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

internal static class UrlEncodingHelper
{
    internal static string UrlEncodeWithCharset(string? charSet, string value)
    {
        Encoding encoding = TextEncodingConverter.GetEncoding(charSet);
        var bytes = encoding.GetBytes(value);

        return StaticStringMethod.Create(bytes.Length * 3, bytes, encodeContent);

        ///////////////////////////////////////////////////////////////

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
