namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Encapsulates the data of an Internet Media Type parameter.
/// </summary>
/// <example>
/// <para>
/// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
/// <para>Formatting a <see cref="MimeType"/> instance into a standards-compliant <see cref="string"/> using several options:</para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FormattingOptionsExample.cs"/>
/// <para>
/// Comparison of <see cref="MimeType"/> instances:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample.cs"/>
/// </example>
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
    internal MimeTypeParameter(string key, string? value, string? language)
    {
        Debug.Assert(key != null);

        // keys are case-insensitive (RFC 2231/7.)
        Key = key.ToLowerInvariant();
        Value = string.IsNullOrEmpty(value) ? null
                                            : IsValueCaseSensitive
                                                 ? value
                                                 : value.Trim().ToLowerInvariant();

        Language = language;
    }

}
