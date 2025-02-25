namespace FolkerKinzel.MimeTypes.Intls.FileTypeExtensions;

internal static class IndexFactory
{
    private const string MIME_INDEX_RESOURCE_NAME = "FolkerKinzel.MimeTypes.Resources.MimeIdx.csv";
    private const string EXTENSION_INDEX_RESOURCE_NAME = "FolkerKinzel.MimeTypes.Resources.ExtensionIdx.csv";

#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
    internal static Dictionary<string, (int, int)> CreateMimeIndex()
#else
    internal static System.Collections.Frozen.FrozenDictionary<string, (int, int)> CreateMimeIndex()
#endif
    {
        using StreamReader reader = ReaderFactory.InitReader(MIME_INDEX_RESOURCE_NAME);

        var dic =
            new Dictionary<string, (int StartIndex, int linesCount)>(16, StringComparer.OrdinalIgnoreCase);
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            const char separator = ' ';

            int separatorIndex1 = line.IndexOf(separator);
            int separatorIndex2 = line.LastIndexOf(separator);

            string mediaType = line.Substring(0, separatorIndex1);

            ++separatorIndex1;
            _ = _Int.TryParse(line.AsSpan(separatorIndex1, separatorIndex2 - separatorIndex1), out int start);

            ++separatorIndex2;
            _ = _Int.TryParse(line.AsSpan(separatorIndex2), out int count);

            dic[mediaType] = (start, count);
        }

#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
        return dic;
#else
        return System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(dic);
#endif
    }

#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
    internal static Dictionary<char, (int, int)> CreateExtensionIndex()
#else
    internal static System.Collections.Frozen.FrozenDictionary<char, (int, int)> CreateExtensionIndex()
#endif
    {
        using StreamReader reader = ReaderFactory.InitReader(EXTENSION_INDEX_RESOURCE_NAME);

        var dic = new Dictionary<char, (int StartIndex, int linesCount)>(32);
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            const char separator = ' ';

            int separatorIndex2 = line.LastIndexOf(separator);

            _ = _Int.TryParse(line.AsSpan(2, separatorIndex2 - 2), out int start);
            _ = _Int.TryParse(line.AsSpan(separatorIndex2 + 1), out int count);

            dic[line[0]] = (start, count);
        }

#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
        return dic;
#else
        return System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(dic);
#endif
    }
}
