namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo : IEquatable<MimeTypeInfo>, ICloneable
{
    /// <summary>
    /// Parses a <see cref="string"/> as <see cref="MimeTypeInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <returns>The <see cref="MimeTypeInfo"/> instance, which <paramref name="value"/> represents.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeTypeInfo"/>.</exception>
    /// <example>
    /// <para>
    /// Build, serialize, and parse a <see cref="MimeTypeInfo"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
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
    /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeTypeInfo"/>.</exception>
    /// <seealso cref="Parse(string)"/>
    /// <seealso cref="TryParse(ReadOnlyMemory{char}, out MimeTypeInfo)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MimeTypeInfo Parse(ReadOnlyMemory<char> value) =>
        ParseInternal(ref value);


    /// <summary>
    /// Tries to parse a <see cref="string"/> as <see cref="MimeTypeInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <param name="mimeType">When the method successfully returns, the parameter contains the
    /// <see cref="MimeTypeInfo"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeTypeInfo"/>; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string? value, out MimeTypeInfo mimeType)
    {
        if (value is null)
        {
            mimeType = default;
            return false;
        }

        ReadOnlyMemory<char> memory = value.AsMemory();
        return TryParseInternal(ref memory, out mimeType);
    }


    /// <summary>
    /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeTypeInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
    /// <param name="mimeType">When the method successfully returns, the parameter contains the
    /// <see cref="MimeTypeInfo"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeTypeInfo"/>; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlyMemory<char> value, out MimeTypeInfo mimeType) =>
        TryParseInternal(ref value, out mimeType);


    /// <summary>
    /// Creates an appropriate <see cref="MimeTypeInfo"/> instance for a given
    /// file name.
    /// </summary>
    /// <param name="fileName">A file path, file name, file type extension (no matter whether with or without the period "."), or <c>null</c>.</param>
    /// <returns>An appropriate <see cref="MimeTypeInfo"/> instance for <paramref name="fileName"/>.</returns>
    /// <remarks>
    /// Internally a small memory cache is used to find often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or You can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if Your application does not need it anymore.
    /// </remarks>
    public static MimeTypeInfo FromFileName(ReadOnlySpan<char> fileName)
    {
        ReadOnlyMemory<char> memory = MimeConverter.FromFileName(fileName).AsMemory();
        _ = TryParseInternal(ref memory, out MimeTypeInfo inetMediaType);
        return inetMediaType;
    }


    /// <summary>
    /// Creates an appropriate <see cref="MimeTypeInfo"/> instance for a given
    /// file name.
    /// </summary>
    /// <param name="fileName">A file path, file name, file type extension (no matter whether with or without the period "."), or <c>null</c>.</param>
    /// <returns>An appropriate <see cref="MimeTypeInfo"/> instance for <paramref name="fileName"/>.</returns>
    /// <remarks>
    /// Internally a small memory cache is used to find often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or You can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if Your application does not need it anymore.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MimeTypeInfo FromFileName(string? fileName) => FromFileName(fileName.AsSpan());
    

}
