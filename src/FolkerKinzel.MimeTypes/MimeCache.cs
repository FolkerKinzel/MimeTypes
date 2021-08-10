using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

#if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.MimeTypes.Intls
{
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
    /// The cache is pre-polated with some of the most frequently used file type extensions and MIME types but it "learns" with every query
    /// and stores after some time only the data he is recently asked for. The cache doesn't exceed a given <see cref="Capacity"/>. The
    /// default value for this is <see cref="DefaultCapacity"/>, which is currently 16, but you can enlarge the <see cref="Capacity"/>
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
        private class Entry
        {
            public Entry(string mimeType, string extension)
            {
                MimeType = mimeType;
                Extension = extension;
            }

            public string MimeType { get; }
            public string Extension { get; }

            [ExcludeFromCodeCoverage]
            public override string ToString() => $"{MimeType} {Extension}";
        }

        ////////////////////////////////////////////

        internal const string DEFAULT_MIME_TYPE = "application/octet-stream";
        internal const string DEFAULT_FILE_TYPE_EXTENSION = "bin";


        private const int CACHE_CLEANUP_SIZE = 4;

        private static readonly object _lockObj = new();
        private static List<Entry>? _mimeCache;
        private static List<Entry>? _extCache;
        private static int _capacity;

        /// <summary>
        /// The default capacity of the cache.
        /// </summary>
        public const int DefaultCapacity = 16;

        /// <summary>
        /// Retrieves the current capacity of the cache.
        /// </summary>
        public static int Capacity => _capacity;


        /// <summary>
        /// Enlarges the <see cref="Capacity"/> of the cache to
        /// the specified value.
        /// </summary>
        /// <param name="newCapacity">The new value for <see cref="Capacity"/>. If <paramref name="newCapacity"/>
        /// is smaller than the current value of <see cref="Capacity"/>, nothing changes.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="newCapacity"/> is smaller than <see cref="DefaultCapacity"/>.</exception>
        public static void EnlargeCapacity(int newCapacity)
        {
            if (newCapacity >= DefaultCapacity)
            {
                lock (_lockObj)
                {
                    if(newCapacity > _capacity)
                    {
                        _capacity = newCapacity;
                    }
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(newCapacity));
            }
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public static void Clear()
        {
            lock (_lockObj)
            {
                _mimeCache = null;
                _extCache = null;
                _capacity = 0;
            }
        }


        internal static string GetMimeType(string? fileTypeExtension)
        {
            if (string.IsNullOrWhiteSpace(fileTypeExtension))
            {
                return DEFAULT_MIME_TYPE;
            }
            else
            {
                fileTypeExtension = fileTypeExtension.Replace(".", null).Replace(" ", null).ToLowerInvariant();
            }

            return TryGetMimeTypeFromCache(fileTypeExtension, out string? mimeType)
                ? mimeType
                : GetMimeTypeFromResources(fileTypeExtension);

            //////////////////////////////////////////

            static bool TryGetMimeTypeFromCache(string fileTypeExtension, [NotNullWhen(true)] out string? mimeType)
            {
                Debug.Assert(!fileTypeExtension.Contains('.'));
                Debug.Assert(!fileTypeExtension.Contains(' '));
                Debug.Assert(!fileTypeExtension.Any(c => char.IsUpper(c)));

                mimeType = default;

                lock (_lockObj)
                {
                    _extCache ??= InitExtCache();

                    if (_extCache.Find(x => x.Extension.Equals(fileTypeExtension, StringComparison.Ordinal)) is Entry entry)
                    {
                        mimeType = entry.MimeType;
                        AddEntryToExtCache(entry);

                        return true;
                    }
                }

                return false;
            }

            static string GetMimeTypeFromResources(string fileTypeExtension)
            {
                string? mimeType = ResourceParser.GetMimeType(fileTypeExtension);
                AddEntryToExtCache(new Entry(mimeType, fileTypeExtension));
                return mimeType;
            }
        }

        internal static string GetFileTypeExtension(string? mimeType)
        {
            return mimeType is null
                ? PrepareFileTypeExtension(DEFAULT_FILE_TYPE_EXTENSION)
                : TryGetFileTypeExtensionFromCache(mimeType, out string? fileTypeExtension)
                        ? PrepareFileTypeExtension(fileTypeExtension)
                        : PrepareFileTypeExtension(GetFileTypeExtensionFromResources(mimeType));

            //////////////////////////////////////////

            static bool TryGetFileTypeExtensionFromCache(string mimeType, [NotNullWhen(true)] out string? fileTypeExtension)
            {
                Debug.Assert(!mimeType.Contains(' '));
                Debug.Assert(!mimeType.Any(c => char.IsUpper(c)));

                fileTypeExtension = default;

                lock (_lockObj)
                {
                    _mimeCache ??= InitMimeCache();

                    if (_mimeCache.Find(x => x.MimeType.Equals(mimeType, StringComparison.Ordinal)) is Entry entry)
                    {
                        fileTypeExtension = entry.Extension;
                        AddEntryToMimeCache(entry);

                        return true;
                    }
                }

                return false;
            }

            static string GetFileTypeExtensionFromResources(string mimeType)
            {
                string fileTypeExtension = ResourceParser.GetFileType(mimeType);
                AddEntryToMimeCache(new Entry(mimeType, fileTypeExtension));
                return fileTypeExtension;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string PrepareFileTypeExtension(string fileTypeExtension) => $".{fileTypeExtension}";



        private static void SetCapacity()
        {
            if(_capacity == 0)
            {
                _capacity = DefaultCapacity;
            }
        }

        private static List<Entry> InitMimeCache()
        {
            SetCapacity();
            return new(_capacity)
            {
                new("application/json", "json"),
                new("application/pdf", "pdf"),
                new("application/rtf", "rtf"),
                new("application/xml", "xml"),
                new("application/zip", "zip"),
                new("image/gif", "gif"),
                new("image/jpeg", "jpg"),
                new("image/png", "png"),
                new("image/svg+xml", "svg"),
                new("message/rfc822", "eml"),
                new("text/html", "htm"),
                new("text/plain", "txt"),
            };
        }

        private static List<Entry> InitExtCache()
        {
            SetCapacity();
            return new(_capacity)
            {
                new("application/json", "json"),
                new("application/pdf", "pdf"),
                new("application/rtf", "rtf"),
                new("application/xml", "xml"),
                new("application/zip", "zip"),
                new("image/gif", "gif"),
                new("image/jpeg", "jpg"),
                new("image/png", "png"),
                new("image/svg+xml", "svg"),
                new("message/rfc822", "eml"),
                new("text/html", "htm"),
                new("text/plain", "txt"),
            };
        }

        private static void AddEntryToExtCache(Entry entry)
        {
            lock (_lockObj)
            {
                _extCache ??= InitExtCache();

                if (_extCache.Capacity < _capacity)
                {
                    _extCache.Capacity = _capacity;
                }

                if (_extCache.Count.Equals(_capacity))
                {
                    _extCache.RemoveRange(0, CACHE_CLEANUP_SIZE);
                }

                _ = _extCache.Remove(entry);
                _extCache.Add(entry);
            }
        }


        private static void AddEntryToMimeCache(Entry entry)
        {
            lock (_lockObj)
            {
                _mimeCache ??= InitMimeCache();

                if (_mimeCache.Capacity < _capacity)
                {
                    _mimeCache.Capacity = _capacity;
                }

                if (_mimeCache.Count.Equals(_capacity))
                {
                    _mimeCache.RemoveRange(0, CACHE_CLEANUP_SIZE);
                }

                _ = _mimeCache.Remove(entry);
                _mimeCache.Add(entry);
            }
        }

    }
}
