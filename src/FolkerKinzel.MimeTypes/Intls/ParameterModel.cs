using FolkerKinzel.MimeTypes.Properties;
using FolkerKinzel.Strings;
using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes.Intls;

/// <summary>
/// Encapsulates and validates the data, which is used to initialize a <see cref="MimeTypeParameter"/>
/// structure.
/// </summary>
internal sealed class ParameterModel
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
    internal ParameterModel(string key, string? value, string? language = null)
    {
        key.ValidateTokenParameter(nameof(key), true);

        if (key.Length > MimeTypeParameter.KEY_LENGTH_MAX_VALUE)
        {
            throw new ArgumentException(Res.StringTooLong, nameof(key));
        }

        Language = string.IsNullOrEmpty(language) ? null : language;
        ValidateLanguageParameter(Language, nameof(language));

        Key = key.Trim();
        Value = string.IsNullOrEmpty(value) ? null : value;
        
    }


    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Key { get; }


    /// <summary>
    /// Gets the value of the parameter.
    /// </summary>
    public string? Value { get; }


    /// <summary>
    /// Gets an IETF-Language tag that indicates the language of the parameter's value.
    /// </summary>
    public string? Language { get; }


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
