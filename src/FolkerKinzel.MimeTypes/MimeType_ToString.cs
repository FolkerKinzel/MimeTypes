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
        /// Creates a complete <see cref="string"/> representation of the instance (according to RFC 2045, RFC 2046 and RFC 2184) that includes the <see cref="Parameters"/>.
        /// </summary>
        /// <returns>A complete <see cref="string"/> representation of the instance (according to RFC 2045, RFC 2046 and RFC 2184) that includes the <see cref="Parameters"/>.</returns>
        public override string ToString() => ToString(MimeTypeFormattingOptions.Default);

        /// <summary>
        /// Creates a <see cref="string"/> representation of the instance, and allows to determine, whether or not to include the
        /// <see cref="Parameters"/>.
        /// </summary>
        /// <param name="options">Named constants to specify options for the serialization of the instance.</param>
        /// <param name="lineLength">The maximum number of characters in a single line of the serialized instance
        /// before a line-wrapping occurs. The parameter is ignored, if the flag <see cref="MimeTypeFormattingOptions.LineWrapping"/>
        /// is not set. If the value of the argument is smaller than <see cref="MinimumLineLength"/>, the value of 
        /// <see cref="MinimumLineLength"/> is taken instead.</param>
        /// <returns>A <see cref="string"/> representation of the instance according to RFC 2045, RFC 2046 and RFC 2184.</returns>
        public string ToString(MimeTypeFormattingOptions options, int lineLength = MinimumLineLength)
        {
            var sb = new StringBuilder(StringLength);
            _ = AppendTo(sb, options, lineLength);
            return sb.ToString();
        }

        /// <summary>
        /// Appends a <see cref="string"/> representation of this instance according to RFC 2045 and RFC 2046 to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/>.</param>
        /// <param name="options">Named constants to specify options for the serialization of the instance.</param>
        /// <param name="lineLength">The maximum number of characters in a single line of the serialized instance
        /// before a line-wrapping occurs. The parameter is ignored, if the flag <see cref="MimeTypeFormattingOptions.LineWrapping"/>
        /// is not set. If the value of the argument is smaller than <see cref="MinimumLineLength"/>, the value of 
        /// <see cref="MinimumLineLength"/> is taken instead.</param>
        /// <returns>A reference to <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public StringBuilder AppendTo(StringBuilder builder, MimeTypeFormattingOptions options, int lineLength = MinimumLineLength)
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

            if (options.HasFlag(MimeTypeFormattingOptions.IncludeParameters))
            {
                bool urlEncodedParameterValues = options.HasFlag(MimeTypeFormattingOptions.AlwaysUrlEncoded);

                if (!urlEncodedParameterValues && options.HasFlag(MimeTypeFormattingOptions.LineWrapping))
                {
                    AppendWrappedParameters(builder, options, lineLength);
                }
                else
                {
                    AppendUnWrappedParameters(builder, options, urlEncodedParameterValues);
                }
            }

            return builder;
        }

        private void AppendUnWrappedParameters(StringBuilder builder, MimeTypeFormattingOptions options, bool urlEncodedParameterValues)
        {
            bool appendSpace = !urlEncodedParameterValues & options.HasFlag(MimeTypeFormattingOptions.WhiteSpaceBetweenParameters);
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

        private void AppendWrappedParameters(StringBuilder builder, MimeTypeFormattingOptions options, int lineLength)
        {
            var worker = new StringBuilder(lineLength);
            bool appendSpace = options.HasFlag(MimeTypeFormattingOptions.WhiteSpaceBetweenParameters);

            foreach (MimeTypeParameter parameter in Parameters)
            {
                _ = parameter.AppendTo(worker.Clear(), false);

                if (worker.Length > lineLength)
                {
                    foreach (StringBuilder tmp in ParameterSplitter.SplitParameter(parameter, worker, lineLength))
                    {
                        _ = builder.Append(';').Append(NEW_LINE).Append(tmp);
                    }
                }
                else
                {
                    _ = builder.Append(';');

                    int neededLength = worker.Length + builder.Length - (builder.LastIndexOf('\n') + 1);
                    
                    if (appendSpace)
                    {
                        neededLength++;
                    }

                    if (neededLength > lineLength)
                    {
                        _ = builder.Append(NEW_LINE);
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
