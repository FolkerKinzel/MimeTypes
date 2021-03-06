using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Properties;
using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System.Text;

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
    public static MimeType Parse(string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(value);
        }

        ReadOnlyMemory<char> memory = value.AsMemory();

        return TryParse(ref memory, out MimeType mediaType)
                ? mediaType
                : throw new ArgumentException(string.Format(Res.InvalidMimeType, nameof(value)), nameof(value));
    }


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

        return TryParse(ref memory, out mimeType);
    }

    /// <summary>
    /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeType"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse. The method might replace the 
    /// passed instance with a smaller one. Make a copy of the argument in the calling method if this is 
    /// not desirable.</param>
    /// <param name="mimeType">When the method successfully returns, the parameter contains the
    /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
    public static bool TryParse(ref ReadOnlyMemory<char> value, out MimeType mimeType)
    {
        value = value.TrimStart();
        ReadOnlySpan<char> span = value.Span;
        int parameterSeparatorIndex = span.IndexOf(';');
        ReadOnlySpan<char> mediaPartSpan = parameterSeparatorIndex < 0 ? span : span.Slice(0, parameterSeparatorIndex);

        // Remove Comment:
        // mediatype/sub.type (Comment)
        int commentStartIndex = mediaPartSpan.IndexOf('(');
        if (commentStartIndex != -1)
        {
            mediaPartSpan = mediaPartSpan.Slice(0, commentStartIndex);
        }

        if (parameterSeparatorIndex < 0)
        {
            // if MimeType has Parameters it must be reallocated
            // (see below)
            mediaPartSpan = mediaPartSpan.TrimEnd();
        }


        // If the mediaPartSpan contains whitespace, repair it:
        if (mediaPartSpan.ContainsWhiteSpace())
        {
            var sb = new StringBuilder(value.Length);
            _ = sb.Append(mediaPartSpan).ReplaceWhiteSpaceWith(ReadOnlySpan<char>.Empty);

            if (parameterSeparatorIndex > 1)
            {
                _ = sb.Append(span.Slice(parameterSeparatorIndex));
            }

            ReadOnlyMemory<char> mem = sb.ToString().AsMemory();
            return TryParse(ref mem, out mimeType);
        }

        int mediaTypeSeparatorIndex = mediaPartSpan.IndexOf('/');

        if (mediaTypeSeparatorIndex < 1) // MediaType must have at least 1 character.
        {
            goto Failed;
        }

        ReadOnlySpan<char> topLevelMediaTypeSpan = mediaPartSpan.Slice(0, mediaTypeSeparatorIndex);

        if (topLevelMediaTypeSpan.Length > MEDIA_TYPE_LENGTH_MAX_VALUE ||
            topLevelMediaTypeSpan.ValidateToken() != TokenError.None)
        {
            goto Failed;
        }


        ReadOnlySpan<char> subTypeSpan = mediaPartSpan.Slice(mediaTypeSeparatorIndex + 1);

        if (subTypeSpan.Length > SUB_TYPE_LENGTH_MAX_VALUE ||
            subTypeSpan.ValidateToken() != TokenError.None)
        {
            goto Failed;
        }

        int idx = parameterSeparatorIndex == -1 ? 0 : 1;
        idx |= subTypeSpan.Length << SUB_TYPE_LENGTH_SHIFT;
        idx |= topLevelMediaTypeSpan.Length << MEDIA_TYPE_LENGTH_SHIFT;

        mimeType = new MimeType(
            in value,
            idx);

        return true;

    /////////////////////////////////////////////////////////////
    Failed:
        mimeType = default;
        return false;
    }


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
        _ = TryParse(ref memory, out MimeType inetMediaType);
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
        _ = TryParse(ref memory, out MimeType inetMediaType);
        return inetMediaType;
    }

}
