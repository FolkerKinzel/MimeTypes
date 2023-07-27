using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType
{
    private ParameterModelDictionary? _dic;

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
    ///  <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public string SubType { get; }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    ///  <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public IEnumerable<MimeTypeParameter> Parameters => _dic?.AsEnumerable() ?? Array.Empty<MimeTypeParameter>();

}
