namespace FolkerKinzel.MimeTypes.Intls.FileTypeExtensions;

/// <summary>
/// Parses MIME types and file type extensions from the resources.
/// </summary>
internal static class ResourceParser
{
    private const char SEPARATOR = ' ';

    // "Reading a Dictionary after population is thread safe." (see
    // https://stackoverflow.com/questions/40593192/c-sharp-when-i-use-only-trygetvalue-on-dictionary-its-thread-safe)
#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
    private static readonly Dictionary<string, (int, int)> _mimeIndex = IndexFactory.CreateMimeIndex();
    private static readonly Dictionary<char, (int, int)> _extensionIndex = IndexFactory.CreateExtensionIndex();
#else
    private static readonly System.Collections.Frozen.FrozenDictionary<string, (int, int)> _mimeIndex
        = IndexFactory.CreateMimeIndex();
    private static readonly System.Collections.Frozen.FrozenDictionary<char, (int, int)> _extensionIndex
        = IndexFactory.CreateExtensionIndex();
#endif

    internal static string GetMimeType(ReadOnlySpan<char> fileTypeExtension)
    {
        Debug.Assert(fileTypeExtension.Length != 0);
        Debug.Assert(fileTypeExtension[0] != '.');

        if (!_extensionIndex.TryGetValue(char.ToLowerInvariant(fileTypeExtension[0]),
                                         out (int Start, int LinesCount) mediaTypeIndex))
        {
            return MimeString.OctetStream;
        }

        using StreamReader reader = ReaderFactory.InitExtensionFileReader();
        reader.BaseStream.Position = mediaTypeIndex.Start;

        for (int i = 0; i < mediaTypeIndex.LinesCount; i++)
        {
            string? line = reader.ReadLine();

            Debug.Assert(line != null);

            int separatorIndex = line.LastIndexOf(SEPARATOR);
            ReadOnlySpan<char> span = line.AsSpan(separatorIndex + 1);

            if (span.Equals(fileTypeExtension, StringComparison.OrdinalIgnoreCase))
            {
                return line.Substring(0, separatorIndex);
            }
        }

        return MimeString.OctetStream;
    }

    internal static string GetFileType(string mimeType)
    {
        if (!_mimeIndex.TryGetValue(GetMediaTypeFromMimeType(mimeType),
                                    out (int Start, int LinesCount) mediaTypeIndex))
        {
            return MimeCache.DEFAULT_EXTENSION_WITHOUT_PERIOD;
        }

        using StreamReader reader = ReaderFactory.InitMimeFileReader();
        reader.BaseStream.Position = mediaTypeIndex.Start;

        ReadOnlySpan<char> mimeSpan = mimeType.AsSpan();

        for (int i = 0; i < mediaTypeIndex.LinesCount; i++)
        {
            string? line = reader.ReadLine();

            Debug.Assert(line != null);

            int separatorIndex = line.LastIndexOf(SEPARATOR);

            ReadOnlySpan<char> span = line.AsSpan(0, separatorIndex);

            if (span.Equals(mimeSpan, StringComparison.OrdinalIgnoreCase))
            {
                return line.Substring(separatorIndex + 1);
            }
        }

        return MimeCache.DEFAULT_EXTENSION_WITHOUT_PERIOD;

        ////////////////////////////////////

        static string GetMediaTypeFromMimeType(string mimeType)
        {
            int sepIdx = mimeType.IndexOf('/');
            return sepIdx == -1 ? mimeType : mimeType.Substring(0, sepIdx);
        }
    }
}
