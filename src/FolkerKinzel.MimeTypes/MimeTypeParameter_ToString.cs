using System;
using System.Linq;
using System.Text;
using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.Strings;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.MimeTypes
{
    public readonly partial struct MimeTypeParameter
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
        /// Appends a <see cref="string"/> representation of this instance according to RFC 2045, RFC 2046 and RFC 2184
        /// to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/>.</param>
        /// <param name="urlEncodedValue">Pass <c>true</c> to URL encode the parameter values.</param>
        /// <returns>A reference to <paramref name="builder"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:In bedingten Ausdruck konvertieren", Justification = "<Ausstehend>")]
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

            return MimeTypeParameterBuilder.Build(builder, in this, urlEncodedValue);

            //ReadOnlySpan<char> valueSpan = Value;

            //bool isValueAscii = valueSpan.IsAscii();
            //bool urlEncoded = urlEncodedValue || !Language.IsEmpty || !isValueAscii;

            //if (urlEncoded)
            //{
            //    MimeTypeParameterBuilder.BuildUrlEncoded(builder, in this, isValueAscii);
            //}
            //else
            //{
            //    // See https://mimesniff.spec.whatwg.org/#serializing-a-mime-type :
            //    // Empty values should be Double-Quoted.
            //    MimeTypeParameterBuilder.BuildQuoted(builder, in this, valueSpan.IsEmpty ? TSpecialKinds.TSpecial : valueSpan.Analyze());
            //}

            //return builder;
        }


    }
}
