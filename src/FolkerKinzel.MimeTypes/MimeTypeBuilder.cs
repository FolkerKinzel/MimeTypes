using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes.Intls;
using static System.Net.Mime.MediaTypeNames;

namespace FolkerKinzel.MimeTypes;

public class MimeTypeBuilder
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
    private MimeTypeBuilder(string mediaType, string subType)
    {
        _mediaType = mediaType ?? throw new ArgumentNullException(nameof(mediaType));
        _subType = subType ?? throw new ArgumentNullException(nameof(subType));
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
    public static MimeTypeBuilder Create(string mediaType, string subType) => new MimeTypeBuilder(mediaType, subType);

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
    public MimeTypeBuilder ClearParameters()
    {
        _dic?.Clear();
        return this;
    }

    /// <summary>
    /// Builds the <see cref="MimeType"/>.
    /// </summary>
    /// <returns>The new <see cref="MimeType"/> instance.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="mediaType"/> or <paramref name="subType"/> is <see cref="string.Empty"/> or is
    /// a <see cref="string"/> that is longer than <see cref="short.MaxValue"/> or contains characters,
    /// which are not permitted by the standard (RFC 2045).
    /// </exception>
    public MimeType Build() => new MimeType(_mediaType, _subType, _dic);


    ///// <summary>
    ///// Indicates whether the struct contains data.
    ///// </summary>
    ///// <value><c>true</c> if the struct contains data, otherwise <c>false</c>.</value>
    //public bool IsEmpty => _mediaType is null;

}
