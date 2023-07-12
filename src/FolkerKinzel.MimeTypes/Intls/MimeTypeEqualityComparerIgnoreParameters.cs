namespace FolkerKinzel.MimeTypes.Intls;

internal sealed class MimeTypeEqualityComparerIgnoreParameters : MimeTypeEqualityComparer
{
    public override bool Equals(MimeType x, MimeType y) => x.Equals(y, true);

    public override int GetHashCode([DisallowNull] MimeType obj) => obj.GetHashCode(true);
}
