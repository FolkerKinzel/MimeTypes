using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameter
{
    [StructLayout(LayoutKind.Auto)]
    private ref struct Indexes
    {
        //private const int KEY_VALUE_OFFSET_MAX_VALUE = MimeTypeParameter.KEY_VALUE_OFFSET_MAX_VALUE;
        //private const int CHARSET_LENGTH_MAX_VALUE = MimeTypeParameter.CHARSET_LENGTH_MAX_VALUE;
        //private const int LANGUAGE_LENGTH_MAX_VALUE = MimeTypeParameter.LANGUAGE_LENGTH_MAX_VALUE;

        public int keyLength;
        public int keyValueOffset;
        public int charsetLength;
        public int languageStart;
        public int languageLength;
        public int valuePartStart;
        public ReadOnlySpan<char> span;


        internal bool Verify()
        {
            if (keyValueOffset > KEY_VALUE_OFFSET_MAX_VALUE || charsetLength > CHARSET_LENGTH_MAX_VALUE || languageLength > LANGUAGE_LENGTH_MAX_VALUE)
            {
                return false;
            }
            return true;
        }

        internal bool VerifyKeyLength() => keyLength is not (0 or > KEY_LENGTH_MAX_VALUE);

        internal void InitCharsetAndLanguage()
        {
            bool inLanguagePart = false;
            for (int i = valuePartStart; i < span.Length; i++)
            {
                char c = span[i];

                if (c == '\'')
                {
                    if (!inLanguagePart)
                    {
                        charsetLength = i - valuePartStart;
                        languageStart = i + 1;
                    }
                    else
                    {
                        languageLength = i - languageStart;
                        break;
                    }

                    inLanguagePart = !inLanguagePart;
                }
            }
        }

    }
}
