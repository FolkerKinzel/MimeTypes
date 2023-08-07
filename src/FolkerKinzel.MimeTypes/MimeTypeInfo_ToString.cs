using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{
    /// <inheritdoc/>
    public override string ToString()
    {
        return base.ToString() ?? nameof(MimeTypeInfo);
    }
}
