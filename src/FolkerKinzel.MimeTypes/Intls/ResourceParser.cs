using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using FolkerKinzel.MimeTypes.Properties;

namespace FolkerKinzel.MimeTypes.Intls
{
    internal static class ResourceParser
    {
        private const char SEPARATOR = ' ';
        
        private static readonly Lazy<ConcurrentDictionary<string, long>> _index = new(IndexFactory.CreateIndex, true);

        internal static string GetMimeType(string fileTypeExtension)
        {
            using StreamReader reader = ReaderFactory.InitExtensionFileReader();

            ReadOnlySpan<char> fileTypeExtensionSpan = fileTypeExtension.AsSpan();
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                int separatorIndex = line.IndexOf(SEPARATOR);
                ReadOnlySpan<char> span = line.AsSpan(separatorIndex + 1);

                if (span.Equals(fileTypeExtensionSpan, StringComparison.Ordinal))
                {
                    return line.Substring(0, separatorIndex);
                }
            }

            return MimeCache.DEFAULT_MIME_TYPE;
        }


        internal static string GetFileType(string mimeType)
        {
            (int Start, int LinesCount) mediaTypeIndex;

            if (_index.Value.TryGetValue(GetMediaTypeFromMimeType(mimeType), out long rawIdx))
            {
                mediaTypeIndex = UnpackIndex(rawIdx);
            }
            else
            {
                return MimeCache.DEFAULT_FILE_TYPE_EXTENSION;
            }

            using StreamReader reader = ReaderFactory.InitMimeFileReader();
            reader.BaseStream.Position = mediaTypeIndex.Start;

            ReadOnlySpan<char> mimeSpan = mimeType.AsSpan();
            for (int i = 0; i < mediaTypeIndex.LinesCount; i++)
            {
                string? line = reader.ReadLine();

                if (line is null)
                {
                    break;
                }

                int separatorIndex = line.IndexOf(SEPARATOR);

                ReadOnlySpan<char> span = line.AsSpan(0, separatorIndex);

                if (span.Equals(mimeSpan, StringComparison.Ordinal))
                {
                    return line.Substring(separatorIndex + 1);
                }
            }

            return MimeCache.DEFAULT_FILE_TYPE_EXTENSION;

            ////////////////////////////////////

            static string GetMediaTypeFromMimeType(string mimeType)
            {
                int sepIdx = mimeType.IndexOf('/');
                return sepIdx == -1 ? mimeType : mimeType.Substring(0, sepIdx);
            }

            static (int Start, int LinesCount) UnpackIndex(long rawIdx)
            {
                int start = (int)(rawIdx & 0xFFFFFFFF);
                int linesCount = (int)(rawIdx >> 32);

                return (start, linesCount);
            }

        }

        

    }
}
