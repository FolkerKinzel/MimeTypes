using System.Collections.Concurrent;

namespace FolkerKinzel.MimeTypes.Intls.FileTypeExtensions;

internal static class IndexFactory
{
    internal static ConcurrentDictionary<string, long> CreateIndex()
    {
        using StreamReader reader = ReaderFactory.InitIndexFileReader();

        var dic = new ConcurrentDictionary<string, long>(Environment.ProcessorCount * 2, 16, StringComparer.OrdinalIgnoreCase);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            const char separator = ' ';

            int separatorIndex1 = line.IndexOf(separator);
            int separatorIndex2 = line.LastIndexOf(separator);

            string mediaType = line.Substring(0, separatorIndex1);

            ++separatorIndex1;
            int start = line.AsSpan(separatorIndex1, separatorIndex2 - separatorIndex1).Parse();

            ++separatorIndex2;
            int count = line.AsSpan(separatorIndex2).Parse();

            dic.TryAdd(mediaType, PackIndex(start, count));
        }

        return dic;
    }


    private static long PackIndex(int start, int linesCount)
    {
        long l = (long)linesCount << 32;
        l |= (long)start;
        return l;
    }
}
