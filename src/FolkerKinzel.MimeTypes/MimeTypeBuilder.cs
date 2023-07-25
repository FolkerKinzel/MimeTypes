using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Fluent API to create <see cref="MimeTypeInfo"/> instances from scratch or to instantiate modified versions of existing MimeTypeInfo instances.
/// </summary>
/// <example>
/// <para>
/// Build, serialize, and parse a <see cref="MimeTypeInfo"/> instance:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
/// </example>
/// <seealso cref="MimeTypeInfo"/>
public sealed class MimeTypeBuilder
{
    private ParameterModelDictionary? _dic;
    private readonly string _mediaType;
    private readonly string _subType;

    /// <summary>
    /// Initializes a new <see cref="MimeTypeBuilder"/> object.
    /// </summary>
    /// <param name="mediaType">The <see cref="MimeTypeInfo.MediaType"/>.</param>
    /// <param name="subType">The <see cref="MimeTypeInfo.SubType"/>.</param>
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
    /// <param name="mediaType">The <see cref="MimeTypeInfo.MediaType"/>.</param>
    /// <param name="subType">The <see cref="MimeTypeInfo.SubType"/>.</param>
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
    /// Build, serialize, and parse a <see cref="MimeTypeInfo"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    /// <seealso cref="MimeTypeInfo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MimeTypeBuilder Create(string mediaType, string subType) => new(mediaType, subType);

    /// <summary>
    /// Creates a new <see cref="MimeTypeBuilder"/> object that's filled with the data of an existing <see cref="MimeTypeInfo"/>
    /// instance.
    /// </summary>
    /// <param name="mime">The <see cref="MimeTypeInfo"/> instance whose data will be copied into the 
    /// <see cref="MimeTypeBuilder"/>.</param>
    /// <returns>A reference to the <see cref="MimeTypeBuilder"/> that is created.</returns>
    public static MimeTypeBuilder Create(in MimeTypeInfo mime)
    {
        var builder = MimeTypeBuilder.Create(mime.MediaType.ToString(), mime.SubType.ToString());

        foreach (var parameter in mime.Parameters())
        {
            var language = parameter.Language;
            _ = builder.AddParameter(parameter.Key.ToString(), parameter.Value.ToString(), language.Length == 0 ? null : language.ToString());
        }

        return builder;
    }

    /// <summary>
    /// Adds a <see cref="MimeTypeParameter"/>.
    /// </summary>
    /// <param name="key">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="language">An IETF-Language tag that indicates the language of the parameter's value.</param>
    /// <returns>A reference to the <see cref="MimeTypeBuilder"/> instance on which the method was called.</returns>
    /// 
    /// <remarks>
    /// The <paramref name="key"/> of a parameter must be unique inside of a <see cref="MimeTypeInfo"/>. It's compared 
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
    /// Build, serialize, and parse a <see cref="MimeTypeInfo"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    /// <seealso cref="MimeTypeParameter"/>
    public MimeTypeBuilder AddParameter(string key, string? value, string? language = null)
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
    /// Removes the <see cref="MimeTypeParameter"/> with the specified <see cref="MimeTypeParameter.Key"/>
    /// from the <see cref="MimeTypeBuilder"/> if such a parameter has existed.
    /// </summary>
    /// <param name="key">The <see cref="MimeTypeParameter.Key"/> of the <see cref="MimeTypeParameter"/> to remove.</param>
    ///  <returns>A reference to the <see cref="MimeTypeBuilder"/> instance on which the method was called.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
    public MimeTypeBuilder RemoveParameter(string key)
    {
        if(key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        _dic?.Remove(key);
        return this;
    }

    /// <summary>
    /// Builds the <see cref="MimeTypeInfo"/>.
    /// </summary>
    /// <returns>The new <see cref="MimeTypeInfo"/> instance.</returns>
    /// <example>
    /// <para>
    /// Build, serialize, and parse a <see cref="MimeTypeInfo"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    /// <seealso cref="MimeTypeInfo"/>
    public MimeTypeInfo Build() => new(_mediaType, _subType, _dic);
}
