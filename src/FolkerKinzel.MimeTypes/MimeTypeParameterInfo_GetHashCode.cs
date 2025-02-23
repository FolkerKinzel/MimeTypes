namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameterInfo
{
    /// <summary>
    /// Computes a hash code for the instance.
    /// </summary>
    /// <returns>The hash code for the instance.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();

        ReadOnlySpan<char> keySpan = Key;
        for (int i = 0; i < keySpan.Length; i++)
        {
            hash.Add(char.ToLowerInvariant(keySpan[i]));
        }

        ReadOnlySpan<char> valueSpan = Value;

        if (IsValueCaseSensitive)
        {
            for (int j = 0; j < valueSpan.Length; j++)
            {
                hash.Add(valueSpan[j]);
            }
        }
        else
        {
            for (int j = 0; j < valueSpan.Length; j++)
            {
                hash.Add(char.ToLowerInvariant(valueSpan[j]));
            }
        }

        return hash.ToHashCode();
    }
}
