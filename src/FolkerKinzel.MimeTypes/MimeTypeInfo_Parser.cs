namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{
    /// <summary>
    /// Parses a <see cref="string"/> as <see cref="MimeTypeInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <returns>The <see cref="MimeTypeInfo"/> instance, which <paramref name="value"/>
    /// represents.</returns>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    /// 
    /// <seealso cref="TryParse(string?, out MimeTypeInfo)"/>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as 
    /// <see cref="MimeTypeInfo"/>.</exception>
    public static MimeTypeInfo Parse(string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(value);
        }

        ReadOnlyMemory<char> memory = value.AsMemory();
        return ParseInternal(ref memory);
    }

    /// <summary>
    /// Parses a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeTypeInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
    /// <returns>The <see cref="MimeTypeInfo"/> instance, which <paramref name="value"/> represents.</returns>
    /// <seealso cref="TryParse(ReadOnlyMemory{char}, out MimeTypeInfo)"/>
    /// 
    /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as 
    /// <see cref="MimeTypeInfo"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MimeTypeInfo Parse(ReadOnlyMemory<char> value) =>
        ParseInternal(ref value);

    /// <summary>
    /// Tries to parse a <see cref="string"/> as <see cref="MimeTypeInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <param name="info">When the method successfully returns, the parameter contains the
    /// <see cref="MimeTypeInfo"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.
    /// </param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeTypeInfo"/>; 
    /// otherwise, <c>false</c>.</returns>
    /// <seealso cref="Parse(string)"/>
    public static bool TryParse(string? value, out MimeTypeInfo info)
    {
        ReadOnlyMemory<char> memory = value.AsMemory();
        return TryParseInternal(ref memory, out info);
    }

    /// <summary>
    /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as 
    /// <see cref="MimeTypeInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see>
    /// to parse.</param>
    /// <param name="info">When the method successfully returns, the parameter contains the
    /// <see cref="MimeTypeInfo"/> parsed from <paramref name="value"/>. The parameter is passed
    /// uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as 
    /// <see cref="MimeTypeInfo"/>; otherwise, <c>false</c>.</returns>
    /// <seealso cref="Parse(ReadOnlyMemory{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlyMemory<char> value, [NotNull] out MimeTypeInfo info)
        => TryParseInternal(ref value, out info);
}
