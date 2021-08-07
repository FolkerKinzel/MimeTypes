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
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        #region ToString

        /// <summary>
        /// Creates a complete <see cref="string"/> representation of the instance (according to RFC 2045 and RFC 2046) that includes the <see cref="Parameters"/>.
        /// </summary>
        /// <returns>A complete <see cref="string"/> representation of the instance (according to RFC 2045 and RFC 2046) that includes the <see cref="Parameters"/>.</returns>
        public override string ToString() => ToString(MimeTypeFormatOptions.Default);

        /// <summary>
        /// Creates a <see cref="string"/> representation of the instance according to RFC 2045 and RFC 2046, and allows to determine, whether or not to include the
        /// <see cref="Parameters"/>.
        /// </summary>
        /// <param name="includeParameters">Pass <c>true</c> to include the <see cref="Parameters"/>; <c>false</c>, otherwise.</param>
        /// <returns>A <see cref="string"/> representation of the instance according to RFC 2045 and RFC 2046.</returns>
        public string ToString(MimeTypeFormatOptions options, int lineLength = MinimumLineLength)
        {
            var sb = new StringBuilder(StringLength);
            _ = AppendTo(sb, options, lineLength);
            return sb.ToString();
        }

        /// <summary>
        /// Appends a <see cref="string"/> representation of this instance according to RFC 2045 and RFC 2046 to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/>.</param>
        /// <param name="includeParameters">Pass <c>true</c> to include the <see cref="Parameters"/>; <c>false</c>, otherwise.</param>
        /// <param name="urlEncodedParameterValues">Pass <c>true</c> to URL encode the parameter values.</param>
        /// <returns>A reference to <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public StringBuilder AppendTo(StringBuilder builder, MimeTypeFormatOptions options, int lineLength = MinimumLineLength)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (IsEmpty)
            {
                return builder;
            }

            if (--lineLength < MinimumLineLength)
            {
                lineLength = MinimumLineLength - 1;
            }

            _ = builder.EnsureCapacity(builder.Length + StringLength);
            int insertStartIndex = builder.Length;
            _ = builder.Append(MediaType).Append('/').Append(SubType).ToLowerInvariant(insertStartIndex);

            if (options.HasFlag(MimeTypeFormatOptions.IncludeParameters))
            {
                bool urlEncodedParameterValues = options.HasFlag(MimeTypeFormatOptions.AlwaysUrlEncoded);

                if (!urlEncodedParameterValues && options.HasFlag(MimeTypeFormatOptions.LineWrapping))
                {
                    AppendWrappedParameters(builder, options, lineLength, insertStartIndex);
                }
                else
                {
                    AppendUnWrappedParameters(builder, options, urlEncodedParameterValues);
                }
            }

            return builder;
        }

        private void AppendUnWrappedParameters(StringBuilder builder, MimeTypeFormatOptions options, bool urlEncodedParameterValues)
        {
            bool appendSpace = !urlEncodedParameterValues & options.HasFlag(MimeTypeFormatOptions.WhiteSpaceBetweenParameters);
            foreach (MimeTypeParameter parameter in Parameters)
            {
                _ = builder.Append(';');
                if (appendSpace)
                {
                    _ = builder.Append(' ');
                }
                _ = parameter.AppendTo(builder, urlEncodedParameterValues);
            }
        }

        private void AppendWrappedParameters(StringBuilder builder, MimeTypeFormatOptions options, int lineLength, int insertStartIndex)
        {
            var worker = new StringBuilder(lineLength);
            bool appendSpace = options.HasFlag(MimeTypeFormatOptions.WhiteSpaceBetweenParameters);

            foreach (MimeTypeParameter parameter in Parameters)
            {
                _ = parameter.AppendTo(worker.Clear(), false);

                if (worker.Length > lineLength)
                {
                    foreach (StringBuilder tmp in ParameterSplitter.SplitParameter(in parameter, worker, lineLength))
                    {
                        _ = builder.Append(';').Append(NEW_LINE);
                        insertStartIndex = builder.Length;
                        _ = builder.Append(tmp);
                    }
                }
                else
                {
                    _ = builder.Append(';');

                    int neededLength = worker.Length;

                    if (appendSpace)
                    {
                        neededLength++;
                    }

                    if (neededLength > lineLength)
                    {
                        _ = builder.Append(NEW_LINE);
                        insertStartIndex = builder.Length;
                    }
                    else if (appendSpace)
                    {
                        _ = builder.Append(' ');
                    }

                    _ = builder.Append(worker);
                }

            }
        }

        #endregion
    }
}
