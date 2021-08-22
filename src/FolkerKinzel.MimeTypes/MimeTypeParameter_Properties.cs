using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes
{
    public readonly partial struct MimeTypeParameter
    {
        private readonly ReadOnlyMemory<char> _parameterString;

        // Stores all indexes in one Int32:
        // | unused |  Language Length | Charset Length | Chars. Indicator | KeyValueOffs | Key Length |
        // | 2 Bit  |      8 Bit       |      8 Bit     |      1 Bit       |    1 Bit     |    12 Bit  |
        private readonly int _idx;

        private const int KEY_LENGTH_MAX_VALUE = 0xFFF;

        private const int KEY_VALUE_OFFSET_SHIFT = 12;
        private const int KEY_VALUE_OFFSET_MAX_VALUE = 1;

        private const int CHARSET_LANGUAGE_INDICATOR_SHIFT = 13;

        private const int CHARSET_LENGTH_SHIFT = 14;
        private const int CHARSET_LENGTH_MAX_VALUE = 0xFF;

        private const int LANGUAGE_LENGTH_SHIFT = 22;
        private const int LANGUAGE_LENGTH_MAX_VALUE = 0xFF;

        // The Offset for the '='-Sign is not stored:
        private int KeyValueOffset => ((_idx >> KEY_VALUE_OFFSET_SHIFT) & KEY_VALUE_OFFSET_MAX_VALUE) + 1;
        private int KeyLength => _idx & KEY_LENGTH_MAX_VALUE;
        private bool ContainsLanguageAndCharset => ((_idx >> CHARSET_LANGUAGE_INDICATOR_SHIFT) & 1) == 1;

        private int CharsetStart => KeyLength + KeyValueOffset;
        private int CharsetLength => (_idx >> CHARSET_LENGTH_SHIFT) & CHARSET_LENGTH_MAX_VALUE;

        private int LanguageStart => KeyLength + KeyValueOffset + CharsetLength + 1;
        private int LanguageLength => (_idx >> LANGUAGE_LENGTH_SHIFT) & LANGUAGE_LENGTH_MAX_VALUE;

        private int ValueStart => ContainsLanguageAndCharset
                                    ? KeyLength + KeyValueOffset + CharsetLength + LanguageLength + 2
                                    : KeyLength + KeyValueOffset;

        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s key.
        /// </summary>
        public ReadOnlySpan<char> Key => _parameterString.Span.Slice(0, KeyLength);

        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s value.
        /// </summary>
        public ReadOnlySpan<char> Value => _parameterString.Span.Slice(ValueStart);

        /// <summary>
        /// The language of <see cref="Value"/>. (IETF-Language tag.)
        /// </summary>
        public ReadOnlySpan<char> Language
        {
            get
            {
                int languageLength = LanguageLength;

                return languageLength == 0
                        ? ReadOnlySpan<char>.Empty
                        : _parameterString.Span.Slice(LanguageStart, languageLength);
            }
        }

        /// <summary>
        /// The charset in which <see cref="Value"/> is encoded.
        /// </summary>
        internal ReadOnlySpan<char> Charset
        {
            get
            {
                int charsetLength = CharsetLength;

                return charsetLength == 0
                    ? ReadOnlySpan<char>.Empty
                    : _parameterString.Span.Slice(CharsetStart, charsetLength);
            }
        }

        /// <summary>
        /// <c>true</c> indicates that the instance contains no data.
        /// </summary>
        public bool IsEmpty => Key.IsEmpty;

        /// <summary>
        /// Returns an empty <see cref="MimeTypeParameter"/> structure.
        /// </summary>
        public static MimeTypeParameter Empty => default;

        
        /// <summary>
        /// Indicates whether the <see cref="MimeTypeParameter"/> has the <see cref="Key"/> "charset". The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if <see cref="Key"/> equals "charset"; otherwise, <c>false</c>.</returns>
        public bool IsCharsetParameter
            => Key.Equals(CHARSET_KEY.AsSpan(), StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// Indicates whether the <see cref="MimeTypeParameter"/> has the <see cref="Key"/> "access-type". The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if <see cref="Key"/> equals "access-type"; otherwise, <c>false</c>.</returns>
        private bool IsAccessTypeParameter
            => Key.Equals("access-type".AsSpan(), StringComparison.OrdinalIgnoreCase);

        internal bool IsValueCaseSensitive => !(IsCharsetParameter || IsAccessTypeParameter);

        /// <summary>
        /// Indicates whether this instance equals "charset=us-ascii". The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if this instance equals "charset=us-ascii"; otherwise, <c>false</c>.</returns>
        public bool IsAsciiCharsetParameter
            => IsCharsetParameter
               && Value.Equals(ASCII_CHARSET_VALUE.AsSpan(), StringComparison.OrdinalIgnoreCase);


    }
}
