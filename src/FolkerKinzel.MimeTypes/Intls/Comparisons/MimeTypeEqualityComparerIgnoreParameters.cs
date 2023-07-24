namespace FolkerKinzel.MimeTypes.Intls.Comparisons;

internal sealed class MimeTypeEqualityComparerIgnoreParameters : MimeTypeEqualityComparer
{
    public override bool Equals(MimeTypeInfo x, MimeTypeInfo y) => x.Equals(y, true);

    public override int GetHashCode([DisallowNull] MimeTypeInfo obj) => obj.GetHashCode(true);
}
