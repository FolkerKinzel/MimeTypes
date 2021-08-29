using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region Operators

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="MimeType"/> instances are equal.
        /// The <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="mimeType1">The first <see cref="MimeType"/> to compare.</param>
        /// <param name="mimeType2">The second <see cref="MimeType"/> to compare.</param>
        /// <returns><c>true</c> if <paramref name="mimeType1"/> and <paramref name="mimeType2"/> are equal;
        /// otherwise, <c>false</c>.</returns>
        /// <example>
        /// <para>
        /// Comparing <see cref="MimeType"/> instances for equality:
        /// </para>
        /// <code source="./../Examples/EqualityExample.cs"/>
        /// </example>
        public static bool operator ==(MimeType mimeType1, MimeType mimeType2) => mimeType1.Equals(in mimeType2, false);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="MimeType"/> instances are not equal.
        /// The <see cref="Parameters"/> are taken into account.
        /// </summary>
        /// <param name="mimeType1">The first <see cref="MimeType"/> to compare.</param>
        /// <param name="mimeType2">The second <see cref="MimeType"/> to compare.</param>
        /// <returns><c>true</c> if <paramref name="mimeType1"/> and <paramref name="mimeType2"/> are not equal;
        /// otherwise, <c>false</c>.</returns>
        /// <example>
        /// <para>
        /// Comparing <see cref="MimeType"/> instances for equality:
        /// </para>
        /// <code language="c#" source="./../Examples/EqualityExample.cs"/>
        /// </example>
        public static bool operator !=(MimeType mimeType1, MimeType mimeType2) => !mimeType1.Equals(in mimeType2, false);

        #endregion


    }
}
