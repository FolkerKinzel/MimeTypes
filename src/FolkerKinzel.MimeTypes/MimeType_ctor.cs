using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Properties;
using System.Runtime.InteropServices;
using System.Text;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Represents a MIME type ("Internet Media Type") according to RFC 2045, RFC 2046 and RFC 2184.
/// </summary>
/// <remarks>
/// <note type="tip">
/// <para>
/// <see cref="MimeType"/> is a quite large structure. Pass it to other methods by reference (in, ref or out parameters in C#)!
/// </para>
/// <para>
/// If you intend to hold a <see cref="MimeType"/> for a long time in memory and if this <see cref="MimeType"/> is parsed
/// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
/// keep in mind, that the <see cref="MimeType"/> holds a reference to that <see cref="string"/>. Consider in this case to make
/// a copy of the <see cref="MimeType"/> structure with <see cref="MimeType.Clone"/>: The copy is built on a separate <see cref="string"/>,
/// which is case-normalized and only as long as needed.
/// </para>
/// </note>
/// </remarks>
/// <example>
/// <para>
/// Getting <see cref="MimeType"/> instances by parsing file type extensions and getting appropriate file type extensions
/// from <see cref="MimeType"/> instances:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
/// <para>
/// Building and parsing <see cref="MimeType"/> instances:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
/// <para>
/// Comparing <see cref="MimeType"/> instances for equality:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample.cs"/>
/// </example>
[StructLayout(LayoutKind.Auto)]
public readonly partial struct MimeType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MimeType"/> structure.
    /// </summary>
    /// <param name="mediaType">The <see cref="MimeType.MediaType"/>.</param>
    /// <param name="subType">The <see cref="MimeType.SubType"/>.</param>
    /// <param name="parameters">The <see cref="MimeType.GetParameters"/> or <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="mediaType"/> or <paramref name="subType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="mediaType"/> or <paramref name="subType"/> is <see cref="string.Empty"/> or is
    /// a <see cref="string"/> that is longer than <see cref="short.MaxValue"/> or contains characters,
    /// which are not permitted by the standard (RFC 2045).
    /// </exception>
    public MimeType(string mediaType, string subType, MimeTypeParameterModelDictionary? parameters = null)
    {
        mediaType.ValidateTokenParameter(nameof(mediaType));
        subType.ValidateTokenParameter(nameof(subType));

        if (mediaType.Length > MEDIA_TYPE_LENGTH_MAX_VALUE)
        {
            throw new ArgumentException(Res.StringTooLong, nameof(mediaType));
        }

        if (mediaType.Length > SUB_TYPE_LENGTH_MAX_VALUE)
        {
            throw new ArgumentException(Res.StringTooLong, nameof(subType));
        }

        _idx = mediaType.Length << MEDIA_TYPE_LENGTH_SHIFT;
        _idx |= subType.Length << SUB_TYPE_LENGTH_SHIFT;

        bool hasParameters = parameters is not null && parameters.Count != 0;

        int capacity = mediaType.Length + subType.Length + 1;

        if (hasParameters)
        {
            _idx |= 1;
            capacity += parameters!.Count * MimeTypeParameter.STRING_LENGTH;
        }

        var sb = new StringBuilder(capacity);
        _ = sb.Append(mediaType).Append('/').Append(subType);

        if (hasParameters)
        {
            for (int i = 0; i < parameters!.Count; i++)
            {
                _ = sb.Append(';');

                MimeTypeParameterModel para = parameters[i];
                ParameterBuilder.Build(sb, in para);
            }
        }

        this._mimeTypeString = sb.ToString().AsMemory();
    }


    private MimeType(in ReadOnlyMemory<char> mimeTypeString, int idx)
    {
        this._mimeTypeString = mimeTypeString;
        this._idx = idx;
    }
}
