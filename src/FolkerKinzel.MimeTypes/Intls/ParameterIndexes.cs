using System;
using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes.Intls;

[SuppressMessage("Style", "IDE1006:Benennungsstile", Justification = "<Ausstehend>")]
[StructLayout(LayoutKind.Auto)]
internal readonly ref struct ParameterIndexes
{
    private const int SEPARATOR_LENGTH = 1;
    private const int LEADING_SINGLE_QUOTE_LENGTH = 1;
    private const int TRAILING_SINGLE_QUOTE_LENGTH = 1;
    private const int STAR_LENGTH = 1;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="span"></param>
    internal ParameterIndexes(ReadOnlySpan<char> span)
    {
        Span = span;
        int keyLengthLocal = span.IndexOf(MimeTypeParameter.SEPARATOR);

        if (!VerifyKeyLength(keyLengthLocal))
        {
            return;
        }

        IsStarred = Span[keyLengthLocal - 1] == '*';
        IsValueQuoted = !IsStarred && GetIsValueQuoted(Span.Slice(keyLengthLocal + 1));
        KeyLength = IsStarred ? keyLengthLocal - STAR_LENGTH : keyLengthLocal;
        KeyValueOffset = IsStarred || IsValueQuoted ? 1 : 0;
        ValuePartStart = KeyLength + KeyValueOffset + SEPARATOR_LENGTH;

        var idx = GetLanguageIdx();
        LanguageStart = idx.LanguageStart;
        LanguageLength = idx.LanguageLength;

        CharsetLength = LanguageStart > 0 ? LanguageStart - LEADING_SINGLE_QUOTE_LENGTH - ValuePartStart
                                          : 0;

        ValueStart = LanguageStart == 0 ? KeyLength
                                        : LanguageStart + 
                                          LanguageLength + 
                                          TRAILING_SINGLE_QUOTE_LENGTH;


        static bool GetIsValueQuoted(ReadOnlySpan<char> valuePartSpan) => 
            valuePartSpan.Length > 1 &&
            valuePartSpan[valuePartSpan.Length - 1] == '\"' && 
            valuePartSpan[0] == '\"';
    }

    /// <summary>
    /// The span to examine.
    /// </summary>
    internal readonly ReadOnlySpan<char> Span;

    /// <summary>
    /// Returns the length of the <see cref="MimeTypeParameter.Key"/> part.
    /// </summary>
    /// <returns></returns>
    internal readonly int KeyLength;

    /// <summary>
    /// <c>1</c> indicates that 1 aditional <see cref="char"/> has to be skipped between <see cref="MimeTypeParameter.Key"/>
    /// and <see cref="MimeTypeParameter.Value"/>.
    /// </summary>
    /// <returns>The number of additional characters that have to be skipped between 
    /// <see cref="MimeTypeParameter.Key"/> and <see cref="MimeTypeParameter.Value"/>.</returns>
    internal readonly int KeyValueOffset;

    /// <summary>
    /// Returns the index where the parameter value part starts (<see cref="MimeTypeParameter.Value"/> 
    /// including <see cref="MimeTypeParameter.CharSet"/> and <see cref="MimeTypeParameter.Language"/>,
    /// if present.
    /// </summary>
    /// <returns>The index where the parameter value starts.</returns>
    /// 
    /// <seealso cref="ValueStart"/>
    internal readonly int ValuePartStart;

    /// <summary>
    /// Gets the length of the CharSet part.
    /// </summary>
    /// <returns>The length of the CharSet part.</returns>
    internal readonly int CharsetLength;

    /// <summary>
    /// Start index of <see cref="MimeTypeParameter.Language"/>.
    /// </summary>
    internal readonly int LanguageStart;

    /// <summary>
    /// Length of <see cref="MimeTypeParameter.Language"/>.
    /// </summary>
    internal readonly int LanguageLength;

    /// <summary>
    /// Returns the index, where <see cref="MimeTypeParameter.Value"/> starts.
    /// </summary>
    /// <returns>The index, where <see cref="MimeTypeParameter.Value"/> starts.</returns>
    internal readonly int ValueStart;

    /// <summary>
    /// Indicates whether the parameter key ends with '*' (and is probably URL encoded).
    /// </summary>
    /// <returns><c>true</c> if the parameter key ends with '*', otherwise false. The return value 
    /// dependens on the state of <see cref="KeyLength"/>.</returns>
    /// <remarks>
    /// <para>
    /// A trailing '*' in the Key indicates that charset and/or language might be present (RFC 2231)
    /// and the value is URL encoded.
    /// If the value is in Double-Quotes, no trailing '*' in the Key is allowed.
    /// </para>
    /// </remarks>
    internal readonly bool IsStarred;

    /// <summary>
    /// Indicates whether the parameter value is enclosed with double quotes.
    /// </summary>
    /// <returns><c>true</c> if the parameter key is enclosed with double quotes, otherwise false.</returns>
    internal readonly bool IsValueQuoted;

    private readonly bool IsSplitted() => Span.Slice(0, KeyLength).IsParameterSplitted();

    internal readonly bool Decode() => IsValueQuoted || !IsSplitted();

    /// <summary>
    /// Verifies the indexes.
    /// </summary>
    /// <returns><c>true</c> if the instance is valid, otherwise <c>false</c>.</returns>
    internal readonly bool Verify() => 
        VerifyKeyLength(KeyLength) &&
        CharsetLength <= MimeTypeParameter.CHARSET_LENGTH_MAX_VALUE &&
        LanguageLength <= MimeTypeParameter.LANGUAGE_LENGTH_MAX_VALUE;

    /// <summary>
    /// Prepares all indexes in a single <see cref="int"/> for the use in the
    /// <see cref="MimeTypeParameter"/> ctor.
    /// </summary>
    /// <returns>All indexes in one <see cref="int"/>.</returns>
    internal readonly int InitMimeTypeParameterCtorIdx()
    {
        int parameterIdx = KeyLength;
        parameterIdx |= KeyValueOffset << MimeTypeParameter.KEY_VALUE_OFFSET_SHIFT;

        if (LanguageStart != 0)
        {
            parameterIdx |= 1 << MimeTypeParameter.CHARSET_LANGUAGE_INDICATOR_SHIFT;
            parameterIdx |= CharsetLength << MimeTypeParameter.CHARSET_LENGTH_SHIFT;
            parameterIdx |= LanguageLength << MimeTypeParameter.LANGUAGE_LENGTH_SHIFT;
        }

        return parameterIdx;
    }

    /// <summary>
    /// Returns <c>true</c> if <see cref="KeyLength"/> is valid.
    /// </summary>
    /// <returns><c>true</c> if <see cref="KeyLength"/> is valid, otherwise <c>false</c>.</returns>
    private static bool VerifyKeyLength(int length) => length is not (< 1 or > MimeTypeParameter.KEY_LENGTH_MAX_VALUE);

    private readonly (int LanguageStart, int LanguageLength) GetLanguageIdx()
    {
        if (!IsStarred)
        {
            return (0,0); 
        }

        int valuePartStart = ValuePartStart;
        bool inLanguage = false;
        int languageStart = 0;
        int languageLength = 0;

        for (int i = valuePartStart; i < Span.Length; i++)
        {
            char c = Span[i];

            if (c == '\'')
            {
                if (!inLanguage)
                {
                    languageStart = i + 1;
                }
                else
                {
                    languageLength = i - languageStart;
                    break;
                }

                inLanguage = !inLanguage;
            }
        }

        return (languageStart, languageLength);
    }

}
