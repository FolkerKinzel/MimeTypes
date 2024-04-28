using FolkerKinzel.MimeTypes.Intls;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType
{
    /// <summary>
    /// Creates a hash code for this instance, which takes the <see cref="Parameters"/> into account.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => GetHashCode(false);


    /// <summary>
    /// Creates a hash code for this instance and allows to specify whether or not
    /// the <see cref="Parameters"/> are taken into account.
    /// </summary>
    /// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> into account; <c>true</c>, otherwise.</param>
    /// <returns>The hash code.</returns>
    /// <seealso cref="MimeTypeParameter"/>
    public int GetHashCode(bool ignoreParameters)
    {
        var hash = new HashCode();
        hash.Add(MediaType, StringComparer.OrdinalIgnoreCase);
        hash.Add(SubType, StringComparer.OrdinalIgnoreCase);

        if (ignoreParameters)
        {
            return hash.ToHashCode();
        }

        foreach (MimeTypeParameter parameter in Parameters.Sort(IsText))
        {
            hash.Add(parameter);
        }

        return hash.ToHashCode();
    }


}
