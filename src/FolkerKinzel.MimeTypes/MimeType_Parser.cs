namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
{
    /// <summary>
    /// Parses a <see cref="string"/> as <see cref="MimeType"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <returns>The <see cref="MimeType"/> instance, which <paramref name="value"/> represents.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeType"/>.</exception>
    /// <example>
    /// <para>
    /// Build, serialize, and parse a <see cref="MimeType"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public static MimeType Parse(string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(value);
        }

        ReadOnlyMemory<char> memory = value.AsMemory();
        return ParseInternal(ref memory);
    }


    /// <summary>
    /// Parses a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeType"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
    /// <returns>The <see cref="MimeType"/> instance, which <paramref name="value"/> represents.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeType"/>.</exception>
    /// <seealso cref="Parse(string)"/>
    /// <seealso cref="TryParse(ReadOnlyMemory{char}, out MimeType)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MimeType Parse(ReadOnlyMemory<char> value) =>
        ParseInternal(ref value);


    /// <summary>
    /// Tries to parse a <see cref="string"/> as <see cref="MimeType"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <param name="mimeType">When the method successfully returns, the parameter contains the
    /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string? value, out MimeType mimeType)
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
    /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeType"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
    /// <param name="mimeType">When the method successfully returns, the parameter contains the
    /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(ReadOnlyMemory<char> value, out MimeType mimeType) =>
        TryParseInternal(ref value, out mimeType);


    /// <summary>
    /// Creates an appropriate <see cref="MimeType"/> instance for a given
    /// file type extension.
    /// </summary>
    /// <param name="fileTypeExtension">The file type extension to search for.</param>
    /// <returns>An appropriate <see cref="MimeType"/> instance for <paramref name="fileTypeExtension"/>.</returns>
    /// <remarks>
    /// Internally a small memory cache is used to find often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or You can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if Your application does not need it anymore.
    /// </remarks>
    /// <example>
    /// <para>
    /// Getting <see cref="MimeType"/> instances by parsing file type extensions and getting appropriate file type extensions
    /// from <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    public static MimeType FromFileTypeExtension(ReadOnlySpan<char> fileTypeExtension)
    {
        ReadOnlyMemory<char> memory = MimeCache.GetMimeType(fileTypeExtension).AsMemory();
        _ = TryParseInternal(ref memory, out MimeType inetMediaType);
        return inetMediaType;
    }


    /// <summary>
    /// Creates an appropriate <see cref="MimeType"/> instance for a given
    /// file type extension.
    /// </summary>
    /// <param name="fileTypeExtension">The file type extension to search for.</param>
    /// <returns>An appropriate <see cref="MimeType"/> instance for <paramref name="fileTypeExtension"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileTypeExtension"/> is <c>null</c>.</exception>
    /// <remarks>
    /// Internally a small memory cache is used to find often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or You can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if Your application does not need it anymore.
    /// </remarks>
    /// <example>
    /// <para>
    /// Getting <see cref="MimeType"/> instances by parsing file type extensions and getting appropriate file type extensions
    /// from <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    public static MimeType FromFileTypeExtension(string fileTypeExtension)
    {
        if (fileTypeExtension is null)
        {
            throw new ArgumentNullException(nameof(fileTypeExtension));
        }

        ReadOnlyMemory<char> memory = MimeCache.GetMimeType(fileTypeExtension).AsMemory();
        _ = TryParseInternal(ref memory, out MimeType inetMediaType);
        return inetMediaType;
    }

}
