using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Fluent API to create <see cref="MimeType"/> instances.
/// </summary>
/// <example>
/// <para>
/// Build, serialize, and parse a <see cref="MimeType"/> instance:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
/// </example>
/// <seealso cref="MimeType"/>
public sealed class MimeTypeBuilder
{
    private ParameterModelDictionary? _dic;
    private readonly string _mediaType;
    private readonly string _subType;

    /// <summary>
    /// Initializes a new <see cref="MimeTypeBuilder"/> object.
    /// </summary>
    /// <param name="mediaType">The <see cref="MimeType.MediaType"/>.</param>
    /// <param name="subType">The <see cref="MimeType.SubType"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="mediaType"/> 
    /// or <paramref name="subType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="mediaType"/> or <paramref name="subType"/> is <see cref="string.Empty"/> or is
    /// a <see cref="string"/> that is longer than <see cref="short.MaxValue"/> or contains characters,
    /// which are not permitted by the standard (RFC 2045).
    /// </exception>
    private MimeTypeBuilder(string mediaType, string subType)
    {
        _mediaType = mediaType ?? throw new ArgumentNullException(nameof(mediaType));
        _subType = subType ?? throw new ArgumentNullException(nameof(subType));
        MimeTypeCtorParametersValidator.Validate(mediaType, subType);
    }


    /// <summary>
    /// Creates a new <see cref="MimeTypeBuilder"/> object.
    /// </summary>
    /// <param name="mediaType">The <see cref="MimeType.MediaType"/>.</param>
    /// <param name="subType">The <see cref="MimeType.SubType"/>.</param>
    /// <returns>A reference to the <see cref="MimeTypeBuilder"/> that is created.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mediaType"/> 
    /// or <paramref name="subType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="mediaType"/> or <paramref name="subType"/> is <see cref="string.Empty"/> or is
    /// a <see cref="string"/> that is longer than <see cref="short.MaxValue"/> or contains characters,
    /// which are not permitted by the standard (RFC 2045).
    /// </exception>
    /// <example>
    /// <para>
    /// Build, serialize, and parse a <see cref="MimeType"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    /// <seealso cref="MimeType"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MimeTypeBuilder Create(string mediaType, string subType) => new(mediaType, subType);


    /// <summary>
    /// Adds a <see cref="MimeTypeParameter"/> to the <see cref="MimeType"/> instance to create.
    /// </summary>
    /// <param name="key">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="language">An IETF-Language tag that indicates the language of the parameter's value.</param>
    /// <returns>A reference to the <see cref="MimeTypeBuilder"/> instance on which the method was called.</returns>
    /// 
    /// <remarks>
    /// The <paramref name="key"/> of a parameter must be unique inside of a <see cref="MimeType"/>. It's compared 
    /// case insensitive. If this method is called several times with equal keys, the last wins.
    /// </remarks>
    /// 
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
    /// <paramref name="language"/> is neither <c>null</c> nor <see cref="string.Empty"/> nor a valid IETF-Language-Tag according to RFC-1766.
    /// </para>
    /// </exception>
    /// <example>
    /// <para>
    /// Build, serialize, and parse a <see cref="MimeType"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    /// <seealso cref="MimeTypeParameter"/>
    public MimeTypeBuilder AppendParameter(string key, string? value, string? language = null)
    {
        _dic ??= new ParameterModelDictionary();
        var model = new ParameterModel(key, value, language);

        _ = _dic.Remove(model.Key);
        _dic.Add(model);

        return this;
    }

    /// <summary>
    /// Removes all <see cref="MimeTypeParameter"/>s.
    /// </summary>
    /// <returns>A reference to the <see cref="MimeTypeBuilder"/> instance on which the method was called.</returns>
    /// <seealso cref="MimeTypeParameter"/>
    public MimeTypeBuilder ClearParameters()
    {
        _dic?.Clear();
        return this;
    }

    /// <summary>
    /// Builds the <see cref="MimeType"/>.
    /// </summary>
    /// <returns>The new <see cref="MimeType"/> instance.</returns>
    /// <example>
    /// <para>
    /// Build, serialize, and parse a <see cref="MimeType"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    /// <seealso cref="MimeType"/>
    public MimeType Build() => new(_mediaType, _subType, _dic);
}
