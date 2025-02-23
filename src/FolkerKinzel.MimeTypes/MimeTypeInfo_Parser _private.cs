using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Properties;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{
    private static MimeTypeInfo ParseInternal(ref ReadOnlyMemory<char> value)
    {
        return TryParseInternal(ref value, out MimeTypeInfo mimeType)
                ? mimeType
                : throw new ArgumentException(string.Format(Res.InvalidMimeType, nameof(value)),
                                              nameof(value));
    }

    private static bool TryParseInternal(ref ReadOnlyMemory<char> value,
                                         out MimeTypeInfo mimeType)
    {
        mimeType = default;

        if (value.IsEmpty)
        {
            return false;
        }

        value = value.TrimStart();
        ReadOnlySpan<char> span = value.Span;
        int parameterSeparatorIndex = span.IndexOf(';');
        ReadOnlySpan<char> mediaPartSpan = parameterSeparatorIndex < 0 
                                           ? span 
                                           : span.Slice(0, parameterSeparatorIndex);
        bool hasParameters = parameterSeparatorIndex > 1; // x/;

        // Remove Comment:
        // mediatype/sub.type (Comment)
        int commentStartIndex = mediaPartSpan.IndexOf('(');
        bool hasComment = false;
        if (commentStartIndex != -1)
        {
            hasComment = true;
            mediaPartSpan = mediaPartSpan.Slice(0, commentStartIndex);
        }

        // if MimeType has Parameters it must be reallocated
        // (see below)
        if (!hasParameters)
        {
            mediaPartSpan = mediaPartSpan.TrimEnd();
        }

        // If the mediaPartSpan contains whitespace, repair it:
        if ((hasParameters && hasComment) || mediaPartSpan.ContainsWhiteSpace())
        {
            return ReAllocate(capacity: value.Length,
                              hasParameters: hasParameters,
                              mediaPartSpan: mediaPartSpan,
                              parameterSpan: hasParameters 
                                              ? span.Slice(parameterSeparatorIndex) 
                                              : [],
                              out mimeType);
        }

        int mediaTypeSeparatorIndex = mediaPartSpan.IndexOf('/');

        if (mediaTypeSeparatorIndex < 1) // MediaType must have at least 1 character.
        {
            return false;
        }

        ReadOnlySpan<char> topLevelMediaTypeSpan = mediaPartSpan.Slice(0, mediaTypeSeparatorIndex);

        if (topLevelMediaTypeSpan.Length > MEDIA_TYPE_LENGTH_MAX_VALUE ||
            !topLevelMediaTypeSpan.IsToken(false))
        {
            return false;
        }

        ReadOnlySpan<char> subTypeSpan = mediaPartSpan.Slice(mediaTypeSeparatorIndex + 1);

        if (subTypeSpan.Length > SUB_TYPE_LENGTH_MAX_VALUE ||
            !subTypeSpan.IsToken(false))
        {
            return false;
        }

        int idx = InitIdx(parameterSeparatorIndex,
                          topLevelMediaTypeSpan.Length,
                          subTypeSpan.Length);
        mimeType = new MimeTypeInfo(in value, idx);
        return true;
    }

    private static bool ReAllocate(int capacity,
                                   bool hasParameters,
                                   ReadOnlySpan<char> mediaPartSpan,
                                   ReadOnlySpan<char> parameterSpan,
                                   out MimeTypeInfo mimeType)
    {
        var sb = new StringBuilder(capacity);
        _ = sb.Append(mediaPartSpan).ReplaceWhiteSpaceWith([]);

        if (hasParameters)
        {
            _ = sb.Append(parameterSpan);
        }

        ReadOnlyMemory<char> mem = sb.ToString().AsMemory();
        return TryParseInternal(ref mem, out mimeType);
    }

    private static int InitIdx(int parameterSeparatorIndex,
                               int topLevelMediaTypeSpanLength,
                               int subTypeSpanLength)
    {
        int idx = parameterSeparatorIndex == -1 ? 0 : 1;
        idx |= subTypeSpanLength << SUB_TYPE_LENGTH_SHIFT;
        idx |= topLevelMediaTypeSpanLength << MEDIA_TYPE_LENGTH_SHIFT;
        return idx;
    }
}
