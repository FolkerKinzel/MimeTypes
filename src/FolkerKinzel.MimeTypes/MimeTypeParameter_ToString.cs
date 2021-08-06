using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.Strings;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.MimeTypes
{
    public readonly partial struct MimeTypeParameter : IEquatable<MimeTypeParameter>, ICloneable
    {
        /// <summary>
        /// Creates a <see cref="string"/> representation of the instance.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of the instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder(StringLength);
            return AppendTo(sb).ToString();
        }

        /// <summary>
        /// Appends a <see cref="string"/> representation of this instance according to RFC 2045 and RFC 2046 to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/>.</param>
        /// <param name="urlEncodedValue">Pass <c>true</c> to URL encode the parameter values.</param>
        /// <returns>A reference to <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public StringBuilder AppendTo(StringBuilder builder, bool urlEncodedValue = false)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // Standard ctor
            if (IsEmpty)
            {
                return builder;
            }


            ReadOnlySpan<char> valueSpan = Value;
            ReadOnlySpan<char> keySpan = Key;
            ReadOnlySpan<char> languageSpan = Language;

            bool isValueAscii = valueSpan.IsAscii();

            bool urlEncoded = !languageSpan.IsEmpty || !isValueAscii;

            // RFC 2045 Section 5.1 "tspecials"
            bool mask = urlEncoded || valueSpan.ContainsAny(stackalloc char[] { ' ', '(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"', '/', '[', '>', ']', '?', '=' });

            if (mask)
            {
                if (urlEncodedValue)
                {
                    urlEncoded = true;
                }

                if (urlEncoded)
                {
                    valueSpan = Uri.EscapeDataString(valueSpan.ToString()).AsSpan();
                    mask = false;
                }
                else if (valueSpan.ContainsAny(stackalloc char[] { '"', '\\' }))
                {
                    var sb = new StringBuilder(valueSpan.Length * 2);
                    _ = sb.Append(valueSpan);
                    valueSpan = Mask(sb).ToString().AsSpan();
                }
            }

            if (mask)
            {
                int neededCapacity = 2 + valueSpan.Length + keySpan.Length + 1;
                _ = builder.EnsureCapacity(builder.Length + neededCapacity);

                int keyStart = builder.Length;
                _ = builder.Append(Key).ToLowerInvariant(keyStart).Append('=');

                _ = builder.Append('\"');

                int valueStart = builder.Length;
                _ = IsValueCaseSensitive
                    ? builder.Append(valueSpan)
                    : builder.Append(valueSpan).ToLowerInvariant(valueStart);

                _ = builder.Append('\"');
            }
            else if (urlEncoded)
            {
                bool starred = !isValueAscii || !languageSpan.IsEmpty;

                const string utf8 = "utf-8";
                int charsetLength = isValueAscii ? 0 : utf8.Length;
                int neededCapacity = valueSpan.Length + keySpan.Length + 1;

                if(starred)
                {
                    neededCapacity += 1 + charsetLength + 2 + languageSpan.Length;
                }

                _ = builder.EnsureCapacity(builder.Length + neededCapacity);

                int keyStart = builder.Length;
                _ = builder.Append(Key).ToLowerInvariant(keyStart);

                _ = starred
                    ? builder.Append('*').Append('=').Append(isValueAscii ? "" : utf8).Append('\'').Append(languageSpan).Append('\'')
                    : builder.Append('=');

                int valueStart = builder.Length;
                _ = IsValueCaseSensitive
                    ? builder.Append(valueSpan)
                    : builder.Append(valueSpan).ToLowerInvariant(valueStart);
            }
            else
            {
                int neededCapacity = valueSpan.Length + keySpan.Length + 1;
                _ = builder.EnsureCapacity(builder.Length + neededCapacity);

                int keyStart = builder.Length;
                _ = builder.Append(Key).ToLowerInvariant(keyStart).Append('=');

                int valueStart = builder.Length;
                _ = IsValueCaseSensitive
                    ? builder.Append(valueSpan)
                    : builder.Append(valueSpan).ToLowerInvariant(valueStart);
            }

            return builder;
        }

        private static StringBuilder Mask(StringBuilder sb)
        {
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                if (sb[i] is '"' or '\\')
                {
                    _ = sb.Insert(i, '\\');
                }
            }

            return sb;
        }
    }
}
