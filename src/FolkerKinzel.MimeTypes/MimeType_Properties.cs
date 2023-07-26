using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType
{
    private ParameterModelDictionary? _dic;

    public string MediaType { get; }

    public string SubType { get; }

    public IEnumerable<MimeTypeParameter> Parameters => _dic?.AsEnumerable() ?? Array.Empty<MimeTypeParameter>();

}
