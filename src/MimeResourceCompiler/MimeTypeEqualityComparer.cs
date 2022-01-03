namespace MimeResourceCompiler;

public class MimeTypeEqualityComparer : IEqualityComparer<Entry>
{
    public bool Equals(Entry? x, Entry? y) => StringComparer.Ordinal.Equals(x?.MimeType, y?.MimeType);
    public int GetHashCode([DisallowNull] Entry obj) => obj.MimeType.GetHashCode();
}
