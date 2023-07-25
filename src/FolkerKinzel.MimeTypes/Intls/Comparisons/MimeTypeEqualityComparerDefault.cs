namespace FolkerKinzel.MimeTypes.Intls.Comparisons;

internal sealed class MimeTypeEqualityComparerDefault : MimeTypeInfoEqualityComparer
{
    public override bool Equals(MimeTypeInfo x, MimeTypeInfo y) => x.Equals(y);

    public override int GetHashCode([DisallowNull] MimeTypeInfo obj) => obj.GetHashCode();
}
