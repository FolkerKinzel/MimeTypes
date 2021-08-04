using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.MimeTypes.Intls
{
    internal static class HelperExtension
    {
        /// <summary>
        /// Converts the whole content of a <see cref="StringBuilder"/> to lower case using the rules
        /// of the invariant culture.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> whose content is modified.</param>
        /// <returns>A reference to <see cref="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        internal static StringBuilder ToLowerInvariant(this StringBuilder builder) 
            => builder is null ? throw new ArgumentNullException(nameof(builder))
                               : builder.ToLowerInvariant(0, builder.Length);

        /// <summary>
        /// Converts the content of a <see cref="StringBuilder"/> beginning at <paramref name="startIndex"/>
        /// to lower case using the rules of the invariant culture.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> whose content is modified.</param>
        /// <param name="startIndex">The zero-based index in <paramref name="builder"/> where the conversion
        /// starts.</param>
        /// <returns>A reference to <see cref="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        internal static StringBuilder ToLowerInvariant(this StringBuilder builder, int startIndex)
            => builder is null ? throw new ArgumentNullException(nameof(builder))
                               : builder.ToLowerInvariant(startIndex, builder.Length - startIndex);

        /// <summary>
        /// Converts a range of chars in a <see cref="StringBuilder"/>, which begins at <paramref name="startIndex"/>
        /// and has the length of <paramref name="count"/> characters, to lower case using the rules of the invariant culture.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> whose content is modified.</param>
        /// <param name="startIndex">The zero-based index in <paramref name="builder"/> where the conversion
        /// starts.</param>
        /// <param name="count">The number of <see cref="char"/>s to convert.</param>
        /// <returns>A reference to <see cref="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        internal static StringBuilder ToLowerInvariant(this StringBuilder builder, int startIndex, int count)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if(startIndex < 0  || startIndex > builder.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if(count < 0 || startIndex + count > builder.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            count += startIndex;
            for (int i = startIndex; i < count; i++)
            {
                char current = builder[i];
                if(char.IsUpper(current))
                {
                    builder[i] = char.ToLowerInvariant(current);
                }
            }

            return builder;
        }

        internal static bool ContainsAny(this ReadOnlySpan<char> span, ReadOnlySpan<char> chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if(span.Contains(chars[i]))
                {
                    return true;
                }
            }

            return false;
        }

        internal static int GetTrimmedLength(this ReadOnlySpan<char> span)
        {
            int length = span.Length;

            for (int i = length-1; i >= 0; i--)
            {
                if (char.IsWhiteSpace(span[i]))
                {
                    length--;
                }
                else
                {
                    break;
                }
            }
            return length;
        }


        internal static int GetTrimmedStart(this ReadOnlySpan<char> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                if (!char.IsWhiteSpace(span[i]))
                {
                    return i;
                }
            }

            return span.Length;
        }

    }
}
