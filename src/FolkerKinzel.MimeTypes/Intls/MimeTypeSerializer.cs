namespace FolkerKinzel.MimeTypes.Intls;

internal static class MimeTypeSerializer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void NormalizeIndexes(StringBuilder builder,
                                                 int startOfMimeType,
                                                 ref int maxLineLength,
                                                 out int startOfCurrentLine)
    {
        startOfCurrentLine = GetCurrentLineStartIndex(builder, startOfMimeType);

        int mimeTypeLength = builder.Length - startOfMimeType;

        if (--maxLineLength < MimeType.MinLineLength)
        {
            maxLineLength = MimeType.MinLineLength - 1;
        }

        if (mimeTypeLength > maxLineLength)
        {
            maxLineLength = mimeTypeLength;
        }

        if (builder.Length - startOfCurrentLine > maxLineLength)
        {
            builder.Insert(startOfMimeType, MimeType.NEW_LINE);
            startOfMimeType += MimeType.NEW_LINE.Length;
            startOfCurrentLine = startOfMimeType;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetCurrentLineStartIndex(StringBuilder builder, int startOfMimeType) => builder.LastIndexOf('\n', startOfMimeType) + 1;

}

