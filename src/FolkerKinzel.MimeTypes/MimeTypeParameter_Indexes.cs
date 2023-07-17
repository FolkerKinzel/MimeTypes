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
        public int KeyLength;
        public int KeyValueOffset;
        public int CharsetLength;
        public int LanguageStart;
        public int LanguageLength;
        public int ValuePartStart;
        public ReadOnlySpan<char> span;


        internal readonly bool Verify() => KeyValueOffset <= KEY_VALUE_OFFSET_MAX_VALUE &&
                                           CharsetLength <= CHARSET_LENGTH_MAX_VALUE &&
                                           LanguageLength <= LANGUAGE_LENGTH_MAX_VALUE;

        internal readonly bool VerifyKeyLength() => KeyLength is not (0 or > KEY_LENGTH_MAX_VALUE);

        internal void InitCharsetAndLanguage()
        {
            bool inLanguagePart = false;
            for (int i = ValuePartStart; i < span.Length; i++)
            {
                char c = span[i];

                if (c == '\'')
                {
                    if (!inLanguagePart)
                    {
                        CharsetLength = i - ValuePartStart;
                        LanguageStart = i + 1;
                    }
                    else
                    {
                        LanguageLength = i - LanguageStart;
                        break;
                    }

                    inLanguagePart = !inLanguagePart;
                }
            }
        }

    }
}
