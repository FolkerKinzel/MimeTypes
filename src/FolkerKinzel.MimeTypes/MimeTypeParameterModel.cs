using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Properties;
using FolkerKinzel.Strings;
using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Encapsulates and validates the data, which is used to initialize a <see cref="MimeTypeParameter"/>
/// structure.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct MimeTypeParameterModel
{
    /// <summary>
    /// Initializes a new <see cref="MimeTypeParameterModel"/> instance.
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
    /// according to RFC 2184,
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
    /// <paramref name="language"/> is neither <c>null</c> nor <see cref="string.Empty"/> nor a valid IETF-Language-Tag according to RFC-1766.
    /// </para>
    /// </exception>
    public MimeTypeParameterModel(string key, string? value, string? language = null)
    {
        key.ValidateTokenParameter(nameof(key));

        if (key.Length > MimeTypeParameter.KEY_LENGTH_MAX_VALUE)
        {
            throw new ArgumentException(Res.StringTooLong, nameof(key));
        }

        ValidateIetfLanguageTag(language, nameof(language));

        Key = key;
        Value = value;
        Language = string.IsNullOrWhiteSpace(language) ? null : language;
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

    /// <summary>
    /// Indicates whether the instance contains no data.
    /// </summary>
    /// <value><c>true</c> if the instance contains no data, otherwise false.</value>
    public bool IsEmpty => Key is null;


    private static void ValidateIetfLanguageTag(string? language, string paraName)
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
