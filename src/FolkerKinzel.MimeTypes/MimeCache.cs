using FolkerKinzel.MimeTypes.Intls.FileTypeExtensions;
using System.Collections.Concurrent;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// A memory cache that's used to retrieve often used file type extensions or MIME types faster.
/// </summary>
/// <threadsafety static="true" instance="true"/>
/// <remarks>
/// <para>
/// It's an expensive operation to parse the resources for MIME types and file type extensions. To overcome this issue, a small
/// cache is is hold in the memory to retrieve the most frequently used MIME types and file type extensions faster.
/// </para>
/// <para>
/// The cache is pre-populated with some of the most frequently used file type extensions and MIME types but it "learns" with every query
/// and stores after some time only the data it is recently asked for. The cache doesn't exceed a given <see cref="Capacity"/>. The
/// default value for this is <see cref="DefaultCapacity"/>, but you can enlarge the <see cref="Capacity"/>
/// with <see cref="EnlargeCapacity(int)"/> if your application uses more than 16 different file types.
/// </para>
/// <para>
/// There is no way to reduce the <see cref="Capacity"/>, but you can <see cref="Clear"/> the whole cache. After that, the next time
/// <see cref="MimeCache"/> is asked for data, it creates a new cache with the <see cref="DefaultCapacity"/> or a larger one, if you 
/// have called <see cref="EnlargeCapacity(int)"/> before.
/// </para>
/// </remarks>
public static class MimeCache
{
    
    internal const string DEFAULT_EXTENSION_WITHOUT_PERIOD = "bin";

    /// <summary>
    /// The file type extension that is used as a fallback value.
    /// </summary>
    public const string DefaultFileTypeExtension = "." + DEFAULT_EXTENSION_WITHOUT_PERIOD;

    private const int CACHE_CLEANUP_SIZE = 4;

    private static int _capacity;

    private static readonly object _lock = new();
    private static Lazy<ConcurrentDictionary<int, string>> _mimeCache = new(CreateMimeCache, true);
    private static Lazy<ConcurrentDictionary<int, (string Extension, string DottedExtension)>> _extCache = new(CreateExtCache, true);


    /// <summary>
    /// The default capacity of the cache.
    /// </summary>
    public const int DefaultCapacity = 16;


    /// <summary>
    /// Gets the current capacity of the cache.
    /// </summary>
    public static int Capacity => _capacity;


    private static void EnsureInitialCapacity() => EnlargeCapacity(DefaultCapacity);


    /// <summary>
    /// Enlarges the <see cref="Capacity"/> of the cache to the specified value.
    /// </summary>
    /// <param name="newCapacity">The new value for <see cref="Capacity"/>. If <paramref name="newCapacity"/>
    /// is smaller than the current value of <see cref="Capacity"/>, nothing changes.</param>
    public static void EnlargeCapacity(int newCapacity)
    {
        if (newCapacity >= DefaultCapacity)
        {
            lock (_lock)
            {
                if (newCapacity > _capacity)
                {
                    _capacity = newCapacity;
                }
            }
        }
    }


    /// <summary>
    /// Clears the cache.
    /// </summary>
    public static void Clear()
    {
        _capacity = 0;
        _mimeCache = new Lazy<ConcurrentDictionary<int, string>>(CreateMimeCache, true);
        _extCache = new Lazy<ConcurrentDictionary<int, (string Extension, string DottedExtension)>>(CreateExtCache, true);
    }


    internal static string GetMimeType(ReadOnlySpan<char> fileTypeExtension)
    {
        fileTypeExtension = fileTypeExtension.Trim();

        if (fileTypeExtension.StartsWith('.'))
        {
            fileTypeExtension = fileTypeExtension.Slice(1);
        }

        return fileTypeExtension.IsWhiteSpace()
                ? MimeString.OctetStream
                : TryGetMimeTypeFromCache(fileTypeExtension, out string? mimeType)
                    ? mimeType
                    : GetMimeTypeFromResources(fileTypeExtension);

        ///////////////////////////////////////////////////////////////
        
        static bool TryGetMimeTypeFromCache(ReadOnlySpan<char> fileTypeExtension, [NotNullWhen(true)] out string? mimeType)
        {
            Debug.Assert(!fileTypeExtension.Contains('.'));
            Debug.Assert(!fileTypeExtension.Contains(' '));

            var cache = _mimeCache.Value;

            return cache.TryGetValue(GetHash(fileTypeExtension), out mimeType);
        }

        //////////////////////////////////////////////////////////////////////////

        static string GetMimeTypeFromResources(ReadOnlySpan<char> fileTypeExtension)
        {
            string mimeType = ResourceParser.GetMimeType(fileTypeExtension);
            AddEntryToMimeCache(fileTypeExtension, mimeType);
            return mimeType;
        }

    }

    internal static string GetFileTypeExtension(string? mimeType, bool leadingDot)
    {
        mimeType = string.IsNullOrWhiteSpace(mimeType)
               ? null
               : mimeType.ReplaceWhiteSpaceWith([]);

        return mimeType is null
            ? leadingDot ? DefaultFileTypeExtension : DEFAULT_EXTENSION_WITHOUT_PERIOD
            : TryGetFileTypeExtensionFromCache(leadingDot, mimeType, out string? fileTypeExtension)
                    ? fileTypeExtension
                    : GetFileTypeExtensionFromResources(mimeType, leadingDot);

        //////////////////////////////////////////

        static bool TryGetFileTypeExtensionFromCache(bool leadingDot, string mimeType, [NotNullWhen(true)] out string? fileTypeExtension)
        {
            Debug.Assert(!mimeType.Contains(' '));

            fileTypeExtension = default;

            var cache = _extCache.Value;

            if(cache.TryGetValue(GetHash(mimeType), out (string Extension, string DottedExtension) value))
            {
                fileTypeExtension = leadingDot ? value.DottedExtension : value.Extension;
                return true;
            }

            return false;
        }
        //////////////////////////////////////////////////////////////////

        static string GetFileTypeExtensionFromResources(string mimeType, bool leadingDot)
        {
            string fileTypeExtension = ResourceParser.GetFileType(mimeType);
            string dottedExtension = $".{fileTypeExtension}";
            AddEntryToExtCache(mimeType, fileTypeExtension, dottedExtension);
            return leadingDot ? dottedExtension : fileTypeExtension;
        }
    }

    private static ConcurrentDictionary<int, string> CreateMimeCache()
    {
        var dic = new ConcurrentDictionary<int, string>();
        dic[GetHash("json")] = "application/json";
        dic[GetHash("pdf")] = "application/pdf";
        dic[GetHash("rtf")] = "application/rtf";
        dic[GetHash("xml")] = "application/xml";
        dic[GetHash("zip")] = "application/zip";
        dic[GetHash("gif")] = "image/gif";
        dic[GetHash("jpg")] = "image/jpeg";
        dic[GetHash("png")] = "image/png";
        dic[GetHash("svg")] = "image/svg+xml";
        dic[GetHash("eml")] = "message/rfc822";
        dic[GetHash("htm")] = "text/html";
        dic[GetHash("txt")] = "text/plain";

        EnsureInitialCapacity();
        return dic;
    }

    private static ConcurrentDictionary<int, (string Extension, string DottedExtension)> CreateExtCache()
    {
        var dic = new ConcurrentDictionary<int, (string Extension, string DottedExtension)>();
        dic[GetHash("application/json")] = ("json", ".json");
        dic[GetHash("application/pdf")] = ("pdf", ".pdf");
        dic[GetHash("application/rtf")] = ("rtf", ".rtf");
        dic[GetHash("application/xml")] = ("xml", ".xml");
        dic[GetHash("application/zip")] = ("zip", ".zip");
        dic[GetHash("image/gif")] = ("gif", ".gif");
        dic[GetHash("image/jpeg")] = ("jpg", ".jpg");
        dic[GetHash("image/png")] = ("png", ".png");
        dic[GetHash("image/svg+xml")] = ("svg", ".svg");
        dic[GetHash("message/rfc822")] = ("eml", ".eml");
        dic[GetHash("text/html")] = ("htm", ".htm");
        dic[GetHash("text/plain")] = ("txt", ".txt");

        EnsureInitialCapacity();
        return dic;
    }


#if NET461 || NETSTANDARD2_0
    private static int GetHash(string? value) => value.AsSpan().GetPersistentHashCode(HashType.OrdinalIgnoreCase);
#endif

    private static int GetHash(ReadOnlySpan<char> value) => value.GetPersistentHashCode(HashType.OrdinalIgnoreCase);


    private static void AddEntryToExtCache(string mimeType, string ext, string dottedExt)
    {
        Debug.Assert(CACHE_CLEANUP_SIZE < DefaultCapacity);

        var cache = _extCache.Value;
        TrimExcess(cache);
        cache[GetHash(mimeType)] = (ext, dottedExt);

        /////////////////////////////////////////////////////////////

        static void TrimExcess(ConcurrentDictionary<int,(string, string)> cache)
        {
            int capacity = Math.Max(_capacity, DefaultCapacity);

            if (cache.Count >= capacity)
            {
                ICollection<int> keys = cache.Keys;

                // another thread could have been changed
                // the Count:
                if (keys.Count >= capacity)
                {
                    foreach (int key in keys.Take(CACHE_CLEANUP_SIZE))
                    {
                        _ = cache.TryRemove(key, out _);
                    }
                }
            }
        }
    }


    private static void AddEntryToMimeCache(ReadOnlySpan<char> ext, string mimeType)
    {
        Debug.Assert(CACHE_CLEANUP_SIZE < DefaultCapacity);

        var cache = _mimeCache.Value;
        TrimExcess(cache);
        cache[GetHash(ext)] = mimeType;

        ////////////////////////////////////////////////////////////

        static void TrimExcess(ConcurrentDictionary<int, string> cache)
        {
            int capacity = Math.Max(_capacity, DefaultCapacity);

            if (cache.Count >= capacity)
            {
                ICollection<int> keys = cache.Keys;

                // another thread could have been changed
                // the Count:
                if (keys.Count >= capacity)
                {
                    foreach (int key in keys.Take(CACHE_CLEANUP_SIZE))
                    {
                        _ = cache.TryRemove(key, out _);
                    }
                }
            }
        }
    }

}
