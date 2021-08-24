﻿using System;
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
    public readonly struct ParameterModel
    {
        /// <summary>
        /// Initializes a new <see cref="ParameterModel"/> instance.
        /// </summary>
        /// <param name="key">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="language">An IETF-Language tag that indicates the language of the parameter's value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <para>
        /// <paramref name="key"/> is <see cref="string.Empty"/> or <paramref name="key"/> is not a valid MIME type parameter name
        /// according to RFC 2184,
        /// </para>
        /// <para>
        /// - or -
        /// </para>
        /// <para>
        /// <paramref name="language"/> is neither <c>null</c> nor <see cref="string.Empty"/> nor a valid IETF-Language-Tag according to RFC-1766.
        /// </para>
        /// </exception>
        public ParameterModel(string key, string? value, string? language = null)
        {
            key.ValidateTokenParameter(nameof(key));
            ValidateIetfLanguageTag(language, nameof(language));

            Key = key;
            Value = value;
            Language = string.IsNullOrWhiteSpace(language) ? null : language;
        }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        public string? Value { get; }

        /// <summary>
        /// An IETF-Language tag that indicates the language of the parameter's value.
        /// </summary>
        public string? Language { get; }

        /// <summary>
        /// <c>true</c> indicates that the instance contains no data.
        /// </summary>
        public bool IsEmpty => Key is null;

        #region private

        private static void ValidateIetfLanguageTag(string? language, string paraName)
        {
            if (language is null)
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

        #endregion
    }
}
