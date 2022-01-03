namespace MimeResourceCompiler;

public class ExtensionEqualityComparer : IEqualityComparer<Entry>
{
    public bool Equals(Entry? x, Entry? y) => StringComparer.Ordinal.Equals(x?.Extension, y?.Extension);
    public int GetHashCode([DisallowNull] Entry obj) => obj.Extension.GetHashCode();
}
