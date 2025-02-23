using FolkerKinzel.MimeTypes.Intls;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{
    /// <summary>
    /// Creates a hash code for this instance, which takes the <see cref="Parameters"/> 
    /// into account.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => GetHashCode(false);

    /// <summary>
    /// Creates a hash code for this instance and allows to specify whether or not
    /// the <see cref="Parameters"/> are taken into account.
    /// </summary>
    /// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> 
    /// into account; <c>true</c>, otherwise.</param>
    /// <returns>The hash code.</returns>
    /// <seealso cref="MimeTypeParameterInfo"/>
    public int GetHashCode(bool ignoreParameters)
    {
        var hash = new HashCode();

        ReadOnlySpan<char> mediaTypeSpan = MediaType;
        for (int i = 0; i < mediaTypeSpan.Length; i++)
        {
            hash.Add(char.ToLowerInvariant(mediaTypeSpan[i]));
        }

        ReadOnlySpan<char> subTypeSpan = SubType;
        for (int j = 0; j < subTypeSpan.Length; j++)
        {
            hash.Add(char.ToLowerInvariant(subTypeSpan[j]));
        }

        if (ignoreParameters)
        {
            return hash.ToHashCode();
        }

        foreach (MimeTypeParameterInfo parameter in Parameters().Sort(IsText))
        {
            hash.Add(parameter);
        }

        return hash.ToHashCode();
    }
}
