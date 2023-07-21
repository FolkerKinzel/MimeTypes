namespace FolkerKinzel.MimeTypes.Intls.Comparisons;

internal sealed class MimeTypeEqualityComparerDefault : MimeTypeEqualityComparer
{
    public override bool Equals(MimeType x, MimeType y) => x.Equals(y);

    public override int GetHashCode([DisallowNull] MimeType obj) => obj.GetHashCode();
}
