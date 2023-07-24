using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;
using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;
using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Represents a MIME type ("Internet Media Type") according to RFC 2045 and RFC 2231.
/// </summary>
/// <remarks>
/// <note type="tip">
/// <para>
/// <see cref="MimeTypeInfo"/> is a quite large structure. Pass it to other methods by reference (in, ref or out parameters in C#)!
/// </para>
/// <para>
/// If you intend to hold a <see cref="MimeTypeInfo"/> for a long time in memory and if this <see cref="MimeTypeInfo"/> is parsed
/// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
/// keep in mind, that the <see cref="MimeTypeInfo"/> holds a reference to that <see cref="string"/>. Consider in this case to make
/// a copy of the <see cref="MimeTypeInfo"/> structure with <see cref="MimeTypeInfo.Clone"/>: The copy is built on a separate <see cref="string"/>,
/// which is case-normalized and only as long as needed.
/// </para>
/// </note>
/// </remarks>
/// <example>
/// <para>
/// Get a <see cref="MimeTypeInfo"/> instance from a file type extension and vice versa:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
/// <para>
/// Build, serialize, and parse a <see cref="MimeTypeInfo"/> instance:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
/// <para>Format a <see cref="MimeTypeInfo"/> instance into a standards-compliant string using several options:</para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FormattingOptionsExample.cs"/>
/// <para>
/// Comparing <see cref="MimeTypeInfo"/> instances for equality:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample.cs"/>
/// </example>
[StructLayout(LayoutKind.Auto)]
public readonly partial struct MimeTypeInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MimeTypeInfo"/> structure.
    /// </summary>
    /// <param name="mediaType">The <see cref="MimeTypeInfo.MediaType"/>.</param>
    /// <param name="subType">The <see cref="MimeTypeInfo.SubType"/>.</param>
    /// <param name="parameters">The <see cref="MimeTypeInfo.Parameters"/> or <c>null</c>.</param>
    /// 
    /// <exception cref="ArgumentException">
    /// <paramref name="mediaType"/> or <paramref name="subType"/> is <see cref="string.Empty"/> or is
    /// a <see cref="string"/> that is longer than <see cref="short.MaxValue"/> or contains characters,
    /// which are not permitted by the standard (RFC 2045).
    /// </exception>
    internal MimeTypeInfo(string mediaType, string subType, ParameterModelDictionary? parameters = null)
    {
        Debug.Assert(mediaType != null);
        Debug.Assert(subType != null);

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

                ParameterModel para = parameters[i];
                ParameterSerializer.Append(sb, in para);
            }
        }

        this._mimeTypeString = sb.ToString().AsMemory();
    }
 

    private MimeTypeInfo(in ReadOnlyMemory<char> mimeTypeString, int idx)
    {
        this._mimeTypeString = mimeTypeString;
        this._idx = idx;
    }
}
