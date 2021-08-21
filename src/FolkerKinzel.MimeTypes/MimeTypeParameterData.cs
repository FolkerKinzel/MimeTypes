using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Properties;
using FolkerKinzel.Strings;

namespace FolkerKinzel.MimeTypes
{
    /// <summary>
    /// Encapsulates and validates the data, which is used to initialize a <see cref="MimeTypeParameter"/>
    /// structure.
    /// </summary>
    public readonly struct MimeTypeParameterData
    {
        /// <summary>
        /// Initializes a new <see cref="MimeTypeParameterData"/> instance.
        /// </summary>
        /// <param name="key">The key of the parameter.</param>
        /// <param name="value">The value of the parameter</param>
        /// <param name="language">The language of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// <paramref name="key"/> is <see cref="string.Empty"/> or is not a valid Mime type parameter name
        /// according to RFC 2184
        /// </para>
        /// <para>
        /// - or -
        /// </para>
        /// <para>
        /// <paramref name="language"/> is neither <c>null</c> nor <see cref="string.Empty"/> nor a valid IETF-Language-Tag according to RFC-1766.
        /// </para>
        /// </exception>
        public MimeTypeParameterData(string key, string? value, string? language = null)
        {
            ValidateKey(key, nameof(key));
            ValidateLanguage(language, nameof(language));

            Key = key;
            Value = value;
            Language = language;
        }

        private static void ValidateLanguage(string? language, string paraName)
        {
            if (string.IsNullOrEmpty(language))
            {
                return;
            }

            for (int i = 0; i < language.Length; i++)
            {
                char current = language[i];

                if (!(char.IsLetter(current) || current == '-') || !current.IsAscii())
                {
                    throw new ArgumentException(string.Format(Res.InvalidIetfLanguageTag, paraName), paraName);
                }
            }
        }

        public string Key { get; }
        public string? Value { get; }
        public string? Language { get; }


        private static void ValidateKey(string key, string paraName)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Length == 0)
            {
                throw new ArgumentException(string.Format(Res.EmptyString, paraName), paraName);
            }

            if (key.ContainsWhiteSpace())
            {
                throw new ArgumentException(string.Format(Res.ContainsWhiteSpace, paraName), paraName);
            }

            for (int i = 0; i < key.Length; i++)
            {
                if (char.IsControl(key[i]))
                {
                    throw new ArgumentException(string.Format(Res.ContainsControlCharacter, paraName), paraName);
                }
            }

            if (key.AsSpan().ContainsTSpecials(out _))
            {
                throw new ArgumentException(string.Format(Res.ContainsTSpecial, paraName), paraName);
            }

            if (!key.IsAscii())
            {
                throw new ArgumentException(string.Format(Res.ContainsNonAscii, paraName), paraName);
            }

            if (key.ContainsAny(stackalloc char[] { '*', '\'', '%' }))
            {
                throw new ArgumentException(string.Format(Res.ContainsReservedCharacter, paraName), paraName);
            }
        }
    }
}
