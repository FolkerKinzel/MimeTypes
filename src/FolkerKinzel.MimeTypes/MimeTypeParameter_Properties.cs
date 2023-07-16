using FolkerKinzel.Strings.Polyfills;
namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameter
{
    private readonly ReadOnlyMemory<char> _parameterString;
    //                                                             Indicates a '*' at the end
    //                                                               of the key or '"' at the 
    // Stores all indexes in one Int32:                                 start of value:
    // | unused |  Language Length | Charset Length | Chars. Indicator | KeyValueOffs | Key Length |
    // | 2 Bit  |      8 Bit       |      8 Bit     |      1 Bit       |    1 Bit     |    12 Bit  |
    private readonly int _idx;

    internal const int KEY_LENGTH_MAX_VALUE = 0xFFF;

    private const int KEY_VALUE_OFFSET_SHIFT = 12;
    private const int KEY_VALUE_OFFSET_MAX_VALUE = 1;

    private const int CHARSET_LANGUAGE_INDICATOR_SHIFT = 13;

    private const int CHARSET_LENGTH_SHIFT = 14;
    private const int CHARSET_LENGTH_MAX_VALUE = 0xFF;

    private const int LANGUAGE_LENGTH_SHIFT = 22;
    internal const int LANGUAGE_LENGTH_MAX_VALUE = 0xFF;

    // The Offset for the '='-Sign is not stored:         *                                         =
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
    /// Gets the <see cref="MimeTypeParameter"/>'s key.
    /// </summary>
    public ReadOnlySpan<char> Key => _parameterString.Span.Slice(0, KeyLength);

    /// <summary>
    /// Gets the <see cref="MimeTypeParameter"/>'s value.
    /// </summary>
    public ReadOnlySpan<char> Value => _parameterString.Span.Slice(ValueStart);

    /// <summary>
    /// Gets an IETF-Language tag that indicates the language of the parameter's value.
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
    /// Gets the character set in which <see cref="Value"/> is encoded.
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
    /// Indicates whether the instance contains no data.
    /// </summary>
    /// <value><c>true</c> if the instance contains no data, otherwise false.</value>
    public bool IsEmpty => _idx == 0;

    /// <summary>
    /// Gets an empty <see cref="MimeTypeParameter"/> structure.
    /// </summary>
    public static MimeTypeParameter Empty => default;


    /// <summary>
    /// Indicates whether the <see cref="MimeTypeParameter"/> has the <see cref="Key"/> "charset". The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if <see cref="Key"/> equals "charset"; otherwise, <c>false</c>.</value>
    public bool IsCharsetParameter
        => Key.Equals(CHARSET_KEY, StringComparison.OrdinalIgnoreCase);


    /// <summary>
    /// Indicates whether the <see cref="MimeTypeParameter"/> has the <see cref="Key"/> "access-type". The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if <see cref="Key"/> equals "access-type"; otherwise, <c>false</c>.</value>
    private bool IsAccessTypeParameter
        => Key.Equals("access-type", StringComparison.OrdinalIgnoreCase);


    /// <summary>
    /// Indicates whether this instance equals "charset=us-ascii". The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance equals "charset=us-ascii"; otherwise, <c>false</c>.</value>
    public bool IsAsciiCharsetParameter
        => IsCharsetParameter
           && Value.Equals(ASCII_CHARSET_VALUE, StringComparison.OrdinalIgnoreCase);


    internal bool IsValueCaseSensitive => !(IsCharsetParameter || IsAccessTypeParameter);

}
