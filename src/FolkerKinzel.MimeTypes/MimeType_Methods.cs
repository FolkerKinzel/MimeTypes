﻿using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

namespace FolkerKinzel.MimeTypes;


public sealed partial class MimeType
{
    /// <summary>
    /// Creates a new <see cref="MimeType"/> object.
    /// </summary>
    /// <param name="mediaType">The <see cref="MimeType.MediaType"/>.</param>
    /// <param name="subType">The <see cref="MimeType.SubType"/>.</param>
    /// <returns>A reference to the <see cref="MimeType"/> that is created.</returns>
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
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample2.cs"/>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MimeType Create(string mediaType, string subType) => new(mediaType, subType);


    /// <summary>
    /// Creates a new <see cref="MimeType"/> object that's filled with the data of an existing <see cref="MimeTypeInfo"/>
    /// instance.
    /// </summary>
    /// <param name="mime">The <see cref="MimeTypeInfo"/> instance whose data will be copied into the 
    /// <see cref="MimeType"/>.</param>
    /// <returns>A reference to the <see cref="MimeType"/> that is created.</returns>
    public static MimeType Create(in MimeTypeInfo mime)
    {
        var builder = MimeType.Create(mime.MediaType.ToString(), mime.SubType.ToString());

        foreach (var parameter in mime.Parameters())
        {
            var language = parameter.Language;
            _ = builder.AppendParameter(parameter.Key.ToString(), parameter.Value.ToString(), language.Length == 0 ? null : language.ToString());
        }

        return builder;
    }

    /// <summary>
    /// Appends a <see cref="MimeTypeParameter"/> to the end of the <see cref="MimeType"/>.
    /// </summary>
    /// <param name="key">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="language">An IETF-Language tag that indicates the language of the parameter's value.</param>
    /// <returns>A reference to the <see cref="MimeType"/> instance on which the method was called.</returns>
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
    /// Build, serialize, and parse a <see cref="MimeType"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample2.cs"/>
    /// </example>
    /// <seealso cref="MimeTypeParameterInfo"/>
    public MimeType AppendParameter(string key, string? value, string? language = null)
    {
        _dic ??= new ParameterModelDictionary();
        var model = new MimeTypeParameter(key, value, language);

        _ = _dic.Remove(model.Key);
        _dic.Add(model);

        return this;
    }

    /// <summary>
    /// Removes all <see cref="MimeTypeParameter"/>s.
    /// </summary>
    /// <returns>A reference to the <see cref="MimeType"/> instance on which the method was called.</returns>
    /// <seealso cref="MimeTypeParameter"/>
    public MimeType ClearParameters()
    {
        _dic?.Clear();
        return this;
    }

    /// <summary>
    /// Removes the <see cref="MimeTypeParameter"/> with the specified <see cref="MimeTypeParameter.Key"/>
    /// from the <see cref="MimeType"/> if such a parameter has existed.
    /// </summary>
    /// <param name="key">The <see cref="MimeTypeParameter.Key"/> of the <see cref="MimeTypeParameter"/> to remove.</param>
    ///  <returns>A reference to the <see cref="MimeType"/> instance on which the method was called.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
    public MimeType RemoveParameter(string key)
    {
        if(key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        _dic?.Remove(key);
        return this;
    }

    /// <summary>
    /// Gets the <see cref="MimeTypeInfo"/> of the <see cref="MimeType"/>s content.
    /// </summary>
    /// <returns>The new <see cref="MimeTypeInfo"/> instance.</returns>
    /// <example>
    /// <para>
    /// Build, serialize, and parse a <see cref="MimeType"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample2.cs"/>
    /// </example>
    /// <seealso cref="MimeTypeInfo"/>
    public MimeTypeInfo AsInfo() => new(MediaType, SubType, _dic);


    /// <summary>
    /// Parses a <see cref="string"/> as <see cref="MimeType"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <returns>The <see cref="MimeType"/> instance that <paramref name="value"/> represents.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeType"/>.</exception>
    /// <example>
    /// <para>
    /// Build, serialize, and parse a <see cref="MimeType"/> instance:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample2.cs"/>
    /// </example>
    public static MimeType Parse(string value) => Create(MimeTypeInfo.Parse(value));


    /// <summary>
    /// Parses a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeType"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
    /// <returns>The <see cref="MimeType"/> instance that <paramref name="value"/> represents.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="MimeType"/>.</exception>
    /// <seealso cref="Parse(string)"/>
    /// <seealso cref="TryParse(ReadOnlyMemory{char}, out MimeType?)"/>
    public static MimeType Parse(ReadOnlyMemory<char> value) => Create(MimeTypeInfo.Parse(value));


    /// <summary>
    /// Tries to parse a <see cref="string"/> as <see cref="MimeType"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <param name="mimeType">When the method successfully returns, the parameter contains the
    /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string? value, [NotNullWhen(true)] out MimeType? mimeType) => TryParse(value.AsMemory(), out mimeType);
    


    /// <summary>
    /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="MimeType"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
    /// <param name="mimeType">When the method successfully returns, the parameter contains the
    /// <see cref="MimeType"/> parsed from <paramref name="value"/>. The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="MimeType"/>; otherwise, <c>false</c>.</returns>
    public static bool TryParse(ReadOnlyMemory<char> value, [NotNullWhen(true)] out MimeType? mimeType)
    {
        mimeType = null;

        if (MimeTypeInfo.TryParse(value, out MimeTypeInfo info))
        {
            mimeType = MimeType.Create(in info);
            return true;
        }

        return false;
    }


    /// <summary>
    /// Creates an appropriate <see cref="MimeType"/> instance for a given
    /// file name.
    /// </summary>
    /// <param name="fileName">A file path, file name, file type extension (no matter whether with or without the period "."), or <c>null</c>.</param>
    /// <returns>An appropriate <see cref="MimeType"/> instance for <paramref name="fileName"/>.</returns>
    /// <remarks>
    /// Internally a small memory cache is used to find often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or You can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if Your application does not need it anymore.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MimeType FromFileName(ReadOnlySpan<char> fileName) => Parse(MimeString.FromFileName(fileName));


    /// <summary>
    /// Creates an appropriate <see cref="MimeType"/> instance for a given
    /// file name.
    /// </summary>
    /// <param name="fileName">A file path, file name, file type extension (no matter whether with or without the period "."), or <c>null</c>.</param>
    /// <returns>An appropriate <see cref="MimeType"/> instance for <paramref name="fileName"/>.</returns>
    /// <remarks>
    /// Internally a small memory cache is used to find often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or You can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if Your application does not need it anymore.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MimeType FromFileName(string? fileName) => FromFileName(fileName.AsSpan());


    private bool IsText => MediaType.Equals("text", StringComparison.OrdinalIgnoreCase);


}