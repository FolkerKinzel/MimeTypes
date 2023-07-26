namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeTypeParameter 
{
    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeParameter"/> instances are equal.
    /// </summary>
    /// <param name="parameter1">The first <see cref="MimeTypeParameter"/> to compare or <c>null</c>.</param>
    /// <param name="parameter2">The second <see cref="MimeTypeParameter"/> to compare or <c>null</c>.</param>
    /// <returns><c>true</c> if <paramref name="parameter1"/> and <paramref name="parameter2"/> are equal;
    /// otherwise, <c>false</c>.</returns>
    public static bool operator ==(MimeTypeParameter? parameter1, MimeTypeParameter? parameter2)
        => parameter1?.Equals(parameter2) ?? parameter2?.Equals(parameter1) ?? true;


    /// <summary>
    /// Returns a value that indicates whether two specified <see cref="MimeTypeParameter"/> instances are not equal.
    /// </summary>
    /// <param name="parameter1">The first <see cref="MimeTypeParameter"/> to compare or <c>null</c>.</param>
    /// <param name="parameter2">The second <see cref="MimeTypeParameter"/> to compare or <c>null</c>.</param>
    /// <returns><c>true</c> if <paramref name="parameter1"/> and <paramref name="parameter2"/> are not equal;
    /// otherwise, <c>false</c>.</returns>
    /// <returns></returns>
    public static bool operator !=(MimeTypeParameter? parameter1, MimeTypeParameter? parameter2)
        => !parameter1?.Equals(parameter2) ?? !parameter2?.Equals(parameter1) ?? false;

}
