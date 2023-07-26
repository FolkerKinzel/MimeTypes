using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;
using FolkerKinzel.MimeTypes.Properties;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Encapsulates the data of an Internet Media Type parameter.
/// </summary>
public sealed partial class MimeTypeParameter 
{
    /// <summary>
    /// Initializes a new <see cref="MimeTypeParameter"/> instance.
    /// </summary>
    /// <param name="key">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="language">An IETF-Language tag that indicates the language of the parameter's value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="key"/> is <see cref="string.Empty"/>
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="key"/> is not a valid MIME type parameter name
    /// according to RFC 2231,
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="key"/> is longer than 4095 characters
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="language"/> is neither <c>null</c> nor empty nor a valid IETF-Language-Tag according to RFC-1766.
    /// </para>
    /// </exception>
    internal MimeTypeParameter(string key, string? value, string? language = null)
    {
        key.ValidateTokenParameter(nameof(key), true);

        if (key.Length > MimeTypeParameterInfo.KEY_LENGTH_MAX_VALUE)
        {
            throw new ArgumentException(Res.StringTooLong, nameof(key));
        }

        Language = string.IsNullOrEmpty(language) ? null : language;
        ValidateLanguageParameter(Language, nameof(language));

        Key = key.Trim();
        Value = string.IsNullOrEmpty(value) ? null : value;

    }

    private static void ValidateLanguageParameter(string? language, string paraName)
    {
        if (language is null)
        {
            return;
        }

        if (!IetfLanguageTag.Validate(language))
        {
            throw new ArgumentException(string.Format(Res.InvalidIetfLanguageTag, paraName), paraName);
        }
    }
}
