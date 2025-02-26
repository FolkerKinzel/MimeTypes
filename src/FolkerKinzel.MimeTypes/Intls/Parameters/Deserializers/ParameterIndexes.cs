﻿using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;
using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers;

[SuppressMessage("Style", "IDE1006:Naming styles", Justification = "<Pending>")]
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

        Starred = Span[keyLengthLocal - 1] == '*';
        IsValueQuoted = !Starred && GetIsValueQuoted(Span.Slice(keyLengthLocal + 1));
        KeyLength = Starred ? keyLengthLocal - STAR_LENGTH : keyLengthLocal;
        KeyValueOffset = Starred || IsValueQuoted ? 1 : 0;
        ValuePartStart = KeyLength + KeyValueOffset + SEPARATOR_LENGTH;

        (int LanguageStart, int LanguageLength) idx = GetLanguageIdx();
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
    /// Returns the length of the <see cref="MimeTypeParameterInfo.Key"/> part.
    /// </summary>
    /// <returns></returns>
    internal readonly int KeyLength;

    /// <summary>
    /// <c>1</c> indicates that 1 aditional <see cref="char"/> has to be skipped between <see cref="MimeTypeParameterInfo.Key"/>
    /// and <see cref="MimeTypeParameterInfo.Value"/>.
    /// </summary>
    /// <returns>The number of additional characters that have to be skipped between 
    /// <see cref="MimeTypeParameterInfo.Key"/> and <see cref="MimeTypeParameterInfo.Value"/>.</returns>
    internal readonly int KeyValueOffset;

    /// <summary>
    /// Returns the index where the parameter value part starts (<see cref="MimeTypeParameterInfo.Value"/> 
    /// including <see cref="MimeTypeParameterInfo.CharSet"/> and <see cref="MimeTypeParameterInfo.Language"/>,
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
    /// Start index of <see cref="MimeTypeParameterInfo.Language"/>.
    /// </summary>
    internal readonly int LanguageStart;

    /// <summary>
    /// Length of <see cref="MimeTypeParameterInfo.Language"/>.
    /// </summary>
    internal readonly int LanguageLength;

    /// <summary>
    /// Returns the index, where <see cref="MimeTypeParameterInfo.Value"/> starts.
    /// </summary>
    /// <returns>The index, where <see cref="MimeTypeParameterInfo.Value"/> starts.</returns>
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
    internal readonly bool Starred;

    /// <summary>
    /// Indicates whether the parameter value is enclosed with double quotes.
    /// </summary>
    /// <returns><c>true</c> if the parameter key is enclosed with double quotes, otherwise false.</returns>
    internal readonly bool IsValueQuoted;

    private readonly bool Splitted() => Span.Slice(0, KeyLength).IsParameterSplitted();

    internal readonly bool Decode() => IsValueQuoted || !Splitted();

    /// <summary>
    /// Verifies the indexes.
    /// </summary>
    /// <returns><c>true</c> if the instance is valid, otherwise <c>false</c>.</returns>
    internal readonly bool Verify() =>
        VerifyKeyLength(KeyLength) &&
        CharsetLength <= MimeTypeParameterInfo.CHARSET_LENGTH_MAX_VALUE &&
        LanguageLength <= MimeTypeParameterInfo.LANGUAGE_LENGTH_MAX_VALUE;

    /// <summary>
    /// Prepares all indexes in a single <see cref="int"/> for the use in the
    /// <see cref="MimeTypeParameterInfo"/> ctor.
    /// </summary>
    /// <returns>All indexes in one <see cref="int"/>.</returns>
    internal readonly int InitMimeTypeParameterCtorIdx()
    {
        int parameterIdx = KeyLength;
        parameterIdx |= KeyValueOffset << MimeTypeParameterInfo.KEY_VALUE_OFFSET_SHIFT;

        if (LanguageStart != 0)
        {
            parameterIdx |= 1 << MimeTypeParameterInfo.CHARSET_LANGUAGE_INDICATOR_SHIFT;
            parameterIdx |= CharsetLength << MimeTypeParameterInfo.CHARSET_LENGTH_SHIFT;
            parameterIdx |= LanguageLength << MimeTypeParameterInfo.LANGUAGE_LENGTH_SHIFT;
        }

        return parameterIdx;
    }

    /// <summary>
    /// Returns <c>true</c> if <see cref="KeyLength"/> is valid.
    /// </summary>
    /// <returns><c>true</c> if <see cref="KeyLength"/> is valid, otherwise <c>false</c>.</returns>
    private static bool VerifyKeyLength(int length)
        => length is not (< 1 or > MimeTypeParameterInfo.KEY_LENGTH_MAX_VALUE);

    private readonly (int LanguageStart, int LanguageLength) GetLanguageIdx()
    {
        if (!Starred)
        {
            return (0, 0);
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
