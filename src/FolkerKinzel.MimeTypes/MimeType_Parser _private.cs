using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Properties;
using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System.Text;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
{
    private static MimeType ParseInternal(ref ReadOnlyMemory<char> value)
    {
        return TryParseInternal(ref value, out MimeType mediaType)
                ? mediaType
                : throw new ArgumentException(string.Format(Res.InvalidMimeType, nameof(value)), nameof(value));
    }


    private static bool TryParseInternal(ref ReadOnlyMemory<char> value, out MimeType mimeType)
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
            return TryParseInternal(ref mem, out mimeType);
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

}
