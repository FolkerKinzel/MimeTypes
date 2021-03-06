using System.Collections.Concurrent;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class IndexFactory
{
    internal static ConcurrentDictionary<string, long> CreateIndex()
    {
        using StreamReader reader = ReaderFactory.InitIndexFileReader();

        var dic = new ConcurrentDictionary<string, long>(Environment.ProcessorCount * 2, 16, StringComparer.Ordinal);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            const char separator = ' ';

            int separatorIndex1 = line.IndexOf(separator);
            int separatorIndex2 = line.LastIndexOf(separator);

            string mediaType = line.Substring(0, separatorIndex1);

            ++separatorIndex1;
#if NETSTANDARD2_0 || NET461
                int start = int.Parse(line.Substring(separatorIndex1, separatorIndex2 - separatorIndex1));
#else
            int start = int.Parse(line.AsSpan(separatorIndex1, separatorIndex2 - separatorIndex1));
#endif
            ++separatorIndex2;

#if NETSTANDARD2_0 || NET461
                int count = int.Parse(line.Substring(separatorIndex2));
#else
            int count = int.Parse(line.AsSpan(separatorIndex2));
#endif

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
