using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType
{
    private ParameterModelDictionary? _dic;

    /// <summary>
    /// Gets the Top-Level Media Type. (The left part of a MIME-Type.)
    /// </summary>
    public string MediaType { get; }

    /// <summary>
    /// Gets the Sub Type. (The right part of a MIME-Type.)
    /// </summary>
    public string SubType { get; }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    public IEnumerable<MimeTypeParameter> Parameters => _dic?.AsEnumerable() ?? Array.Empty<MimeTypeParameter>();

}
