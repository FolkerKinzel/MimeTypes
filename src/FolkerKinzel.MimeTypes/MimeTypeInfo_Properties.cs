using FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{
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
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    public ReadOnlySpan<char> MediaType => _mimeTypeString.Span.Slice(0, MediaTypeLength);

    /// <summary>
    /// Gets the Sub Type. (The right part of a MIME-Type.)
    /// </summary>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    public ReadOnlySpan<char> SubType
        => IsEmpty ? [] : _mimeTypeString.Span.Slice(MediaTypeLength + 1, SubTypeLength);

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <returns>The collection of parameters of the <see cref="MimeTypeInfo"/>.</returns>
    /// <remarks>
    /// <note type="tip">Iterating through the <see cref="MimeTypeParameterInfo"/>s can be an 
    /// expensive operation in some cases. Consider to call 
    /// <see cref="Enumerable.ToArray{TSource}(IEnumerable{TSource})"/> on the return value 
    /// if you need it more than once.</note></remarks>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    public IEnumerable<MimeTypeParameterInfo> Parameters() =>
        HasParameters
        ? ParameterParser.ParseParameters(_mimeTypeString.Slice(MediaTypeLength + SubTypeLength + 2))
        : [];

    /// <summary>
    /// Gets an empty <see cref="MimeTypeInfo"/> structure.
    /// </summary>
    public static MimeTypeInfo Empty => default;

    /// <summary>
    /// Indicates whether the instance contains no data.
    /// </summary>
    /// <value><c>true</c> if the instance contains no data, otherwise false.</value>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    public bool IsEmpty => _idx == 0;

    /// <summary>
    /// Determines whether the <see cref="MediaType"/> of this instance equals "text".
    /// The comparison is case-insensitive.
    /// </summary>
    /// <returns><c>true</c> if the <see cref="MediaType"/> of this instance equals "text".
    /// </returns>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    public bool IsText
        => _mimeTypeString.Span.StartsWith("text/", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Indicates whether this instance is equal to the MIME type "text/plain". The parameters 
    /// are not taken into account. The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance is equal to "text/plain".</value>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    public bool IsTextPlain
        => _mimeTypeString.Span.StartsWith("text/plain", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Indicates whether this instance is equal to <see cref="MimeString.OctetStream"/>. The 
    /// parameters are not taken into account. The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance is equal to <see cref="MimeString.OctetStream"/>, 
    /// otherwise <c>false</c>.</value>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    public bool IsOctetStream
        => _mimeTypeString.Span.StartsWith(MimeString.OctetStream, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets an appropriate file type extension for the <see cref="MimeTypeInfo"/> instance.
    /// </summary>
    /// <param name="includePeriod"><c>true</c> specifies, that the period "." (U+002E) is included
    /// in the retrieved file type extension, <c>false</c>, that it's not.</param>
    /// <returns>An appropriate file type extension for the <see cref="MimeTypeInfo"/> instance.</returns>
    /// <remarks>
    /// <para>
    /// If no other file type extension can be found, <see cref="MimeCache.DefaultFileTypeExtension"/>
    /// is returned. <paramref name="includePeriod"/> specifies whether the period is included.
    /// </para>
    /// <para>
    /// Internally a small memory cache is used to retrieve often used file type extensions faster. You
    /// can enlarge the size of this cache with 
    /// <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or you can delete it 
    /// with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if your application does not need it anymore.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    public string GetFileTypeExtension(bool includePeriod = true)
        => MimeString.ToFileTypeExtension(_mimeTypeString.Span, includePeriod);
}
