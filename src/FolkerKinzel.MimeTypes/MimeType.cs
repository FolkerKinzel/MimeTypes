namespace FolkerKinzel.MimeTypes;

public static class MimeType
{
    public static string GetFileTypeExtension(string? mimeType, bool leadingDot = true)
    {
        if (mimeType != null)
        {
            int parameterStartIdx = mimeType.IndexOf(';');

            if (parameterStartIdx >= 0)
            {
                mimeType = mimeType.Substring(0, parameterStartIdx);
            }

            mimeType = string.IsNullOrWhiteSpace(mimeType)
                ? null
                : mimeType.ReplaceWhiteSpaceWith(ReadOnlySpan<char>.Empty).ToLowerInvariant();
        }
        return MimeCache.GetFileTypeExtension(mimeType, leadingDot);
    }

    public static string GetFileTypeExtension(ReadOnlySpan<char> mimeType, bool leadingDot = true)
    {
        int parameterStartIdx = mimeType.IndexOf(';');

        if (parameterStartIdx >= 0)
        {
            mimeType = mimeType.Slice(0, parameterStartIdx);
        }

        string? mimeTypeString = mimeType.IsWhiteSpace()
            ? null
            : mimeType.ToString().ReplaceWhiteSpaceWith(ReadOnlySpan<char>.Empty).ToLowerInvariant();

        return MimeCache.GetFileTypeExtension(mimeTypeString, leadingDot);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromFileTypeExtension(string? fileTypeExtension) => MimeCache.GetMimeType(fileTypeExtension);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromFileTypeExtension(ReadOnlySpan<char> fileTypeExtension) => MimeCache.GetMimeType(fileTypeExtension);
}
