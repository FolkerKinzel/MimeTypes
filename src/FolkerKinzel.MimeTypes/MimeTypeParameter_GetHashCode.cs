namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeTypeParameter
{
    /// <summary>
    /// Computes a hash code for the instance.
    /// </summary>
    /// <returns>The hash code for the instance.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Key, StringComparer.OrdinalIgnoreCase);

        if (IsValueCaseSensitive)
        {
            hash.Add(Value, StringComparer.Ordinal);
        }
        else
        {
            hash.Add(Value, StringComparer.OrdinalIgnoreCase);
        }

        return hash.ToHashCode();
    }

}
