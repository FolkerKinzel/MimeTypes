using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes
{
    public readonly partial struct MimeTypeParameter : IEquatable<MimeTypeParameter>, ICloneable, IComparable<MimeTypeParameter>
    {
        /// <summary>
        /// Compares the current instance with another <see cref="MimeTypeParameter"/> and returns an integer that indicates whether 
        /// the current instance precedes, follows, or occurs in the same position in the sort order as the other <see cref="MimeTypeParameter"/>.
        /// </summary>
        /// <param name="other">The other <see cref="MimeTypeParameter"/> instance to compare with.</param>
        /// <returns>A value that indicates the relative order of the instances being compared.</returns>
        public int CompareTo(MimeTypeParameter other) => Key.CompareTo(other.Key, StringComparison.OrdinalIgnoreCase);
    }
}
