using FolkerKinzel.MimeTypes.Intls;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Encapsulates the data of an Internet Media Type ("MIME type").
/// </summary>
/// <example>
/// <para>
/// Build, serialize, and parse a <see cref="MimeType"/> instance:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample2.cs"/>
/// </example>
/// <seealso cref="MimeTypeInfo"/>
public sealed partial class MimeType 
{
    /// <summary>
    /// Initializes a new <see cref="MimeType"/> object.
    /// </summary>
    /// <param name="mediaType">The <see cref="MimeType.MediaType"/>.</param>
    /// <param name="subType">The <see cref="MimeType.SubType"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="mediaType"/> 
    /// or <paramref name="subType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="mediaType"/> or <paramref name="subType"/> is <see cref="string.Empty"/> or is
    /// a <see cref="string"/> that is longer than <see cref="short.MaxValue"/> or contains characters,
    /// which are not permitted by the standard (RFC 2045).
    /// </exception>
    private MimeType(string mediaType, string subType)
    {
        MediaType = mediaType ?? throw new ArgumentNullException(nameof(mediaType));
        SubType = subType ?? throw new ArgumentNullException(nameof(subType));
        MimeTypeCtorParametersValidator.Validate(mediaType, subType);
    }
}
