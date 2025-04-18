﻿using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType
{
    private ParameterModelDictionary? _dic;

    [MemberNotNullWhen(true, nameof(_dic))]
    private bool HasParameters => _dic is not null && _dic.Count != 0;

    /// <summary>
    /// Gets the Top-Level Media Type. (The left part of a MIME-Type.)
    /// </summary>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public string MediaType { get; }

    /// <summary>
    /// Gets the Sub Type. (The right part of a MIME-Type.)
    /// </summary>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public string SubType { get; }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public IEnumerable<MimeTypeParameter> Parameters => _dic?.AsEnumerable() ?? [];

    /// <summary>
    /// Determines whether the <see cref="MediaType"/> of this instance equals "text".
    /// The comparison is case-insensitive.
    /// </summary>
    /// <returns><c>true</c> if the <see cref="MediaType"/> of this instance equals "text".</returns>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public bool IsText
        => MediaType.Equals("text", StringComparison.Ordinal);

    /// <summary>
    /// Indicates whether this instance is equal to the MIME type "text/plain". The parameters 
    /// are not taken into account. The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance is equal to "text/plain".</value>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public bool IsTextPlain => IsText && SubType.Equals("plain", StringComparison.Ordinal);

    /// <summary>
    /// Indicates whether this instance is equal to <see cref="MimeString.OctetStream"/>. The 
    /// parameters are not taken into account. The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance is equal to <see cref="MimeString.OctetStream"/>, 
    /// otherwise <c>false</c>.</value>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public bool IsOctetStream
        => MediaType.Equals("application", StringComparison.OrdinalIgnoreCase)
           && SubType.Equals("octet-stream", StringComparison.Ordinal);
}
