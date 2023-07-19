using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes.Intls;

[SuppressMessage("Style", "IDE1006:Benennungsstile", Justification = "<Ausstehend>")]
[StructLayout(LayoutKind.Auto)]
internal ref struct ParameterIndexes
{
    private const int SEPARATOR_LENGTH = 1;

    internal int KeyLength;
    internal int KeyValueOffset;
    internal int CharsetLength;
    internal int LanguageStart;
    internal int LanguageLength;

    /// <summary>
    /// Gets the index where the parameter value starts.
    /// </summary>
    /// <returns>The index where the parameter value starts.</returns>
    /// <remarks>
    /// <note type="caution">
    /// The method is stateless: Its return value depends on the values of
    /// <see cref="KeyLength"/> and <see cref="KeyValueOffset"/> !!!
    /// </note>
    /// </remarks>
    internal readonly int ValuePartStart() => KeyLength + KeyValueOffset + SEPARATOR_LENGTH;
    //                                                                        =



    internal readonly ReadOnlySpan<char> Span;


    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="span"></param>
    internal ParameterIndexes(ReadOnlySpan<char> span) => Span = span;


    internal readonly bool Verify() => 
        VerifyKeyLength() &&
        KeyValueOffset <= MimeTypeParameter.KEY_VALUE_OFFSET_MAX_VALUE &&
        CharsetLength <= MimeTypeParameter.CHARSET_LENGTH_MAX_VALUE &&
        LanguageLength <= MimeTypeParameter.LANGUAGE_LENGTH_MAX_VALUE;


    internal readonly bool VerifyKeyLength() => KeyLength is not (0 or > MimeTypeParameter.KEY_LENGTH_MAX_VALUE);


    /// <summary>
    /// Indicates whether the parameter key ends with '*' (and is probably URL encoded).
    /// </summary>
    /// <returns><c>true</c> if the parameter key ends with '*', otherwise false. The return value 
    /// dependens on the state of <see cref="KeyLength"/>.</returns>
    /// <remarks>
    /// <para>
    /// A trailing '*' in the Key indicates that charset and/or language might be present (RFC 2184)
    /// and the value is URL encoded.
    /// If the value is in Double-Quotes, no trailing '*' in the Key is allowed.
    /// </para>
    /// <para>
    /// <note type="caution">
    /// The method is stateless: Its return value depends on the conditions under which the method is called.
    /// </note>
    /// </para>
    /// </remarks>
    internal readonly bool ContainsCharSetAndLanguage() => KeyLength > 0 && Span[KeyLength - 1] == '*';


    /// <summary>
    /// Indicates whether the parameter value is enclosed with double quotes.
    /// </summary>
    /// <returns><c>true</c> if the parameter key is enclosed with double quotes, otherwise false.</returns>
    internal readonly bool IsValueQuoted()
    {
        int valuePartStart = ValuePartStart();
        int spanLastIndex = Span.Length - 1;
        return Span[spanLastIndex] == '\"' && spanLastIndex > valuePartStart && Span[valuePartStart] == '\"';
    }


    internal readonly int GetValueStart()
    {
        //If the Key is thekey* at least two single quotes for the Language are present:
        //                    '               '                                       
        return LanguageStart + LanguageLength + (LanguageStart == 0 ? 0 : 1);
    }

    internal readonly int InitCtorIdx()
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

}
