using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes
{
    public readonly partial struct MimeTypeParameter : IComparable<MimeTypeParameter>
    {
        /// <summary>
        /// Compares the current instance with another <see cref="MimeTypeParameter"/> and returns an integer that indicates whether 
        /// the current instance precedes, follows, or occurs in the same position in the sort order as the other <see cref="MimeTypeParameter"/>.
        /// </summary>
        /// <param name="other">The other <see cref="MimeTypeParameter"/> instance to compare with.</param>
        /// <returns>A value that indicates the relative order of the instances being compared.</returns>
        /// <remarks>The method takes only the <see cref="MimeTypeParameter.Key"/>s into account.</remarks>
        public int CompareTo(MimeTypeParameter other) => Key.CompareTo(other.Key, StringComparison.OrdinalIgnoreCase);
    }
}
