﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Properties;
using FolkerKinzel.Strings;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.MimeTypes
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region Parser

        /// <summary>
        /// Parses a <see cref="string"/> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <returns>The <see cref="MimeType"/> instance, which <paramref name="value"/> represents.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeType"/>.</exception>
        public static MimeType Parse(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(value);
            }

            ReadOnlyMemory<char> memory = value.AsMemory();

            return TryParse(ref memory, out MimeType mediaType)
                    ? mediaType
                    : throw new ArgumentException(string.Format(Res.InvalidMimeType, nameof(value)), nameof(value));
        }


        /// <summary>
        /// Tries to parse a <see cref="string"/> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <param name="mimeType">When the method successfully returns, the parameter contains the
        /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string? value, out MimeType mimeType)
        {
            if (value is null)
            {
                mimeType = default;
                return false;
            }

            ReadOnlyMemory<char> memory = value.AsMemory();

            return TryParse(ref memory, out mimeType);
        }

        /// <summary>
        /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeType"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse. The method might replace the 
        /// passed instance with a smaller one. Make a copy of the argument in the calling method if this is 
        /// not desirable.</param>
        /// <param name="mimeType">When the method successfully returns, the parameter contains the
        /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
        public static bool TryParse(ref ReadOnlyMemory<char> value, out MimeType mimeType)
        {
            value = value.TrimStart();
            ReadOnlySpan<char> span = value.Span;

            int parameterSeparatorIndex = span.IndexOf(';');

            if (parameterSeparatorIndex > PARAMETERS_START_MAX_VALUE) // string too long
            {
                goto Failed;
            }

            ReadOnlySpan<char> mediaPartSpan = parameterSeparatorIndex < 0 ? span : span.Slice(0, parameterSeparatorIndex);
            
            // Remove Comment:
            // mediatype/sub.type (Comment)
            int commentStartIndex = mediaPartSpan.IndexOf('(');
            if(commentStartIndex != -1)
            {
                mediaPartSpan = mediaPartSpan.Slice(0, commentStartIndex);
            }

            int mediaTypeSeparatorIndex = mediaPartSpan.IndexOf('/');

            if (mediaTypeSeparatorIndex < 1)
            {
                goto Failed;
            }

            int topLevelMediaTypeLength = mediaPartSpan.Slice(0, mediaTypeSeparatorIndex).GetTrimmedLength();

            if (topLevelMediaTypeLength is 0 or > MEDIA_TYPE_LENGTH_MAX_VALUE)
            {
                goto Failed;
            }

            int subTypeStart = mediaTypeSeparatorIndex + 1;
            subTypeStart += mediaPartSpan.Slice(subTypeStart).GetTrimmedStart();
            int subTypeLength = mediaPartSpan.Slice(subTypeStart).GetTrimmedLength();

            if (subTypeLength == 0 || subTypeStart > SUB_TYPE_START_MAX_VALUE)
            {
                goto Failed;
            }

            int idx = topLevelMediaTypeLength << MEDIA_TYPE_LENGTH_SHIFT;
            idx |= subTypeStart << SUB_TYPE_START_SHIFT;
            idx |= subTypeLength << SUB_TYPE_LENGTH_SHIFT;
            idx |= parameterSeparatorIndex < 0 ? value.Length : parameterSeparatorIndex + 1;

            mimeType = new MimeType(
                in value,
                idx);

            return true;

/////////////////////////////////////////////////////////////
Failed:
            mimeType = default;
            return false;
        }

        /// <summary>
        /// Creates an appropriate <see cref="MimeType"/> instance for a given
        /// file type extension.
        /// </summary>
        /// <param name="fileTypeExtension">The file type extension to search for.</param>
        /// <returns>An appropriate <see cref="MimeType"/> instance for <paramref name="fileTypeExtension"/>.</returns>
        public static MimeType FromFileTypeExtension(string? fileTypeExtension)
        {
            ReadOnlyMemory<char> memory = MimeCache.GetMimeType(fileTypeExtension).AsMemory();
            _ = TryParse(ref memory, out MimeType inetMediaType);
            return inetMediaType;
        }

        #endregion

    }
}
