using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes.Intls;

[SuppressMessage("Style", "IDE1006:Benennungsstile", Justification = "<Ausstehend>")]
[StructLayout(LayoutKind.Auto)]
internal ref struct ParameterIndexes
{
    private const int SEPARATOR_LENGTH = 1;
    private const int KEY_LENGTH_MAX_VALUE = MimeTypeParameter.KEY_LENGTH_MAX_VALUE;
    private const int KEY_VALUE_OFFSET_MAX_VALUE = MimeTypeParameter.KEY_VALUE_OFFSET_MAX_VALUE;
    private const int CHARSET_LENGTH_MAX_VALUE = MimeTypeParameter.CHARSET_LENGTH_MAX_VALUE;
    private const int LANGUAGE_LENGTH_MAX_VALUE = MimeTypeParameter.LANGUAGE_LENGTH_MAX_VALUE;

    internal int KeyLength;
    internal int KeyValueOffset;
    internal int CharsetLength;
    internal int LanguageStart;
    internal int LanguageLength;
    //                                                                        =
    internal readonly int ValuePartStart => KeyLength + KeyValueOffset + SEPARATOR_LENGTH;
    internal readonly ReadOnlySpan<char> Span;


    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="span"></param>
    internal ParameterIndexes(ReadOnlySpan<char> span) => Span = span;


    internal readonly bool Verify() => 
        VerifyKeyLength() &&
        KeyValueOffset <= KEY_VALUE_OFFSET_MAX_VALUE &&
        CharsetLength <= CHARSET_LENGTH_MAX_VALUE &&
        LanguageLength <= LANGUAGE_LENGTH_MAX_VALUE;


    internal readonly bool VerifyKeyLength() => KeyLength is not (0 or > KEY_LENGTH_MAX_VALUE);


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
        int spanLastIndex = Span.Length - 1;
        return Span[spanLastIndex] == '\"' && spanLastIndex > ValuePartStart && Span[ValuePartStart] == '\"';
    }


    internal readonly int GetValueStart()
    {
        //If the Key is thekey* at least two single quotes for the Language are present:
        //                    '               '                                       
        return LanguageStart + LanguageLength + (LanguageStart == 0 ? 0 : 1);
    }



}
