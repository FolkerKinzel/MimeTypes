using FolkerKinzel.Strings.Polyfills;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
{
    private const int MEDIA_TYPE_LENGTH_MAX_VALUE = short.MaxValue;
    private const int SUB_TYPE_LENGTH_MAX_VALUE = short.MaxValue;
    private const int SUB_TYPE_LENGTH_SHIFT = 1;
    private const int MEDIA_TYPE_LENGTH_SHIFT = 16;

    private readonly ReadOnlyMemory<char> _mimeTypeString;

    // Stores all indexes in one int.
    // | unused |     MediaTp Length    |  SubType Length  |  Contains Parameters  |
    // |  1 Bit |        15 Bit         |      15 Bit      |        1 Bit          |
    private readonly int _idx;


    private bool HasParameters => (_idx & 1) == 1;

    private int SubTypeLength => (_idx >> SUB_TYPE_LENGTH_SHIFT) & SUB_TYPE_LENGTH_MAX_VALUE;

    private int MediaTypeLength => (_idx >> MEDIA_TYPE_LENGTH_SHIFT) & MEDIA_TYPE_LENGTH_MAX_VALUE;

    /// <summary>
    /// Gets the Top-Level Media Type. (The left part of a MIME-Type.)
    /// </summary>
    public ReadOnlySpan<char> MediaType => _mimeTypeString.Span.Slice(0, MediaTypeLength);

    /// <summary>
    /// Gets the Sub Type. (The right part of a MIME-Type.)
    /// </summary>
    public ReadOnlySpan<char> SubType
        => IsEmpty ? ReadOnlySpan<char>.Empty : _mimeTypeString.Span.Slice(MediaTypeLength + 1, SubTypeLength);

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <returns>The collection of parameters of the <see cref="MimeType"/>.</returns>
    /// <remarks>Iterating through the <see cref="MimeTypeParameter"/>s can be an expensive operation
    /// under certain circumstances. Consider to call <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/>
    /// on the return value, if you need it more than once.</remarks>
    public IEnumerable<MimeTypeParameter> GetParameters() => ParseParameters();

    /// <summary>
    /// Indicates whether the instance contains no data.
    /// </summary>
    /// <value><c>true</c> if the instance contains no data, otherwise false.</value>
    public bool IsEmpty => _idx == 0;

    /// <summary>
    /// Gets an empty <see cref="MimeType"/> structure.
    /// </summary>
    public static MimeType Empty => default;

    /// <summary>
    /// Finds an appropriate file type extension for the <see cref="MimeType"/> instance.
    /// </summary>
    /// <returns>An appropriate file type extension for the <see cref="MimeType"/> instance.</returns>
    /// <remarks>
    /// Internally a small memory cache is used to retrieve often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or You can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if Your application does not need it anymore.
    /// </remarks>
    public string GetFileTypeExtension()
        => MimeCache.GetFileTypeExtension(IsEmpty ? null : ToString(MimeTypeFormattingOptions.None));

    /// <summary>
    /// Finds an appropriate file type extension for <paramref name="mimeTypeString"/>.
    /// </summary>
    /// <param name="mimeTypeString">A <see cref="string"/> that represents an Internet Media Type ("MIME type")
    /// according to RFC 2045, RFC 2046 and RFC 2184.</param>
    /// <returns>An appropriate file type extension for <paramref name="mimeTypeString"/>.</returns>
    /// <remarks>
    /// Internally a small memory cache is used to retrieve often used file type extensions faster. You
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
    public static string GetFileTypeExtension(string? mimeTypeString)
    {
        _ = TryParse(mimeTypeString, out MimeType mimeType);
        return mimeType.GetFileTypeExtension();
    }

    /// <summary>
    /// Determines whether the <see cref="MediaType"/> of this instance equals "text".
    /// The comparison is case-insensitive.
    /// </summary>
    /// <returns><c>true</c> if the <see cref="MediaType"/> of this instance equals "text".</returns>
    public bool IsText
        => MediaType.Equals("text", StringComparison.OrdinalIgnoreCase);


    /// <summary>
    /// Indicates whether this instance is equal to the MIME type "text/plain". The parameters are not taken into account.
    /// The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance is equal to "text/plain".</value>
    public bool IsTextPlain
        => IsText && SubType.Equals("plain", StringComparison.OrdinalIgnoreCase);

}
