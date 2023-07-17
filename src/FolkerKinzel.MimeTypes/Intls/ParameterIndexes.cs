using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Intls;

[StructLayout(LayoutKind.Auto)]
internal ref struct ParameterIndexes
{
    private const int KEY_LENGTH_MAX_VALUE = MimeTypeParameter.KEY_LENGTH_MAX_VALUE;
    private const int KEY_VALUE_OFFSET_MAX_VALUE = MimeTypeParameter.KEY_VALUE_OFFSET_MAX_VALUE;
    private const int CHARSET_LENGTH_MAX_VALUE = MimeTypeParameter.CHARSET_LENGTH_MAX_VALUE;
    private const int LANGUAGE_LENGTH_MAX_VALUE = MimeTypeParameter.LANGUAGE_LENGTH_MAX_VALUE;

    public int KeyLength;
    public int KeyValueOffset;
    public int CharsetLength;
    public int LanguageStart;
    public int LanguageLength;
    public int ValuePartStart;
    public bool? starred;

    public readonly ReadOnlySpan<char> Span;

    public ParameterIndexes(ReadOnlySpan<char> span) => Span = span;


    internal readonly bool Verify() => 
        VerifyKeyLength() &&
        KeyValueOffset <= KEY_VALUE_OFFSET_MAX_VALUE &&
        CharsetLength <= CHARSET_LENGTH_MAX_VALUE &&
        LanguageLength <= LANGUAGE_LENGTH_MAX_VALUE;

    internal readonly bool VerifyKeyLength() => KeyLength is not (0 or > KEY_LENGTH_MAX_VALUE);

    internal void InitUrlEncodedOffsets()
    {
        --KeyLength; // Eat the trailing '*'.
        ++KeyValueOffset;
        InitCharsetAndLanguage();
    }

    /// <summary>
    /// Indicates whether the parameter key end with '*'.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A trailing '*' in the Key indicates that charset and/or language might be present (RFC 2184)
    /// and the value is URL encoded.
    /// If the value is in Double-Quotes, no trailing '*' in the Key is allowed.
    /// </para>
    /// <para>
    /// The method memorizes its first answer and returns <c>true</c> in a second call even if the trailing
    /// '*' has been removed.
    /// </para>
    /// </remarks>
    internal bool IsValueUrlEncoded()
    {
        starred ??= KeyLength > 0 && Span[KeyLength - 1] == '*';
        return starred.Value;
    }

    internal readonly bool IsValueQuoted()
    {
        int spanLastIndex = Span.Length - 1;
        return Span[spanLastIndex] == '\"' && spanLastIndex > ValuePartStart && Span[ValuePartStart] == '\"';
    }

    private void InitCharsetAndLanguage()
    {
        bool inLanguagePart = false;
        for (int i = ValuePartStart; i < Span.Length; i++)
        {
            char c = Span[i];

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
