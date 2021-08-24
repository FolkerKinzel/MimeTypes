using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using FolkerKinzel.MimeTypes.Intls;
using static System.Net.Mime.MediaTypeNames;

namespace FolkerKinzel.MimeTypes
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        private const int MEDIA_TYPE_LENGTH_MAX_VALUE = short.MaxValue;
        private const int SUB_TYPE_LENGTH_MAX_VALUE = short.MaxValue;
        private const int SUB_TYPE_LENGTH_SHIFT = 1;
        private const int MEDIA_TYPE_LENGTH_SHIFT = 16;

        private readonly ReadOnlyMemory<char> _mimeTypeString;

        // Stores all indexes in one int.
        // | unused |     MediaTp Length    |  SubType Length  |  Contains Parameters  |
        // |  1 Bit |        15 Bit         |      15 Bit      |        1 Bit          |
        private readonly int _idx;


        #region Properties

        private bool HasParameters => (_idx & 1) == 1;

        private int SubTypeLength => (_idx >> SUB_TYPE_LENGTH_SHIFT) & SUB_TYPE_LENGTH_MAX_VALUE;

        private int MediaTypeLength => (_idx >> MEDIA_TYPE_LENGTH_SHIFT) & MEDIA_TYPE_LENGTH_MAX_VALUE;

        /// <summary>
        /// Top-Level Media Type. (The left part of a MIME-Type.)
        /// </summary>
        public ReadOnlySpan<char> MediaType => _mimeTypeString.Span.Slice(0, MediaTypeLength);

        /// <summary>
        /// Sub Type (The right part of a MIME-Type.)
        /// </summary>
        public ReadOnlySpan<char> SubType
            => IsEmpty ? ReadOnlySpan<char>.Empty : _mimeTypeString.Span.Slice(MediaTypeLength + 1, SubTypeLength);

        /// <summary>
        /// Parameters (Never <c>null</c>.)
        /// </summary>
        public IEnumerable<MimeTypeParameter> Parameters => ParseParameters();

        /// <summary>
        /// <c>true</c> if the instance contains no data.
        /// </summary>
        public bool IsEmpty => _idx == 0;

        /// <summary>
        /// Returns a <see cref="MimeType"/> structure, which contains no data.
        /// </summary>
        public static MimeType Empty => default;

        /// <summary>
        /// Finds an appropriate file type extension for the <see cref="MimeType"/> instance.
        /// </summary>
        /// <returns>An appropriate file type extension for the <see cref="MimeType"/> instance.</returns>
        public string GetFileTypeExtension()
            => MimeCache.GetFileTypeExtension(IsEmpty ? null : ToString(MimeTypeFormattingOptions.None));

        /// <summary>
        /// Finds an appropriate file type extension for <paramref name="mimeTypeString"/>.
        /// </summary>
        /// <param name="mimeTypeString">A <see cref="string"/> that represents an Internet Media Type ("MIME type")
        /// according to RFC 2045, RFC 2046 and RFC 2184.</param>
        /// <returns>An appropriate file type extension for <paramref name="mimeTypeString"/>.</returns>
        public static string GetFileTypeExtension(string? mimeTypeString)
        {
            _ = TryParse(mimeTypeString, out MimeType mimeType);
            return mimeType.GetFileTypeExtension();
        }

        /// <summary>
        /// Determines whether the <see cref="MediaType"/> of this instance equals "text".
        /// The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="MediaType"/> of this instance equals "text".</returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool IsText
            => MediaType.Equals("text".AsSpan(), StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// Determines whether this instance is equal to the MIME type "text/plain". The parameters are not taken into account.
        /// The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if this instance is equal to "text/plain".</returns>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool IsTextPlain
            => IsText && SubType.Equals("plain".AsSpan(), StringComparison.OrdinalIgnoreCase);



        #endregion

    }
}
