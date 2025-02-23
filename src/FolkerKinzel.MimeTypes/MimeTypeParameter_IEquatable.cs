namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeTypeParameter : IEquatable<MimeTypeParameter>
{
    /// <summary>
    /// Determines if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.
    /// </summary>
    /// <param name="other">A <see cref="MimeTypeParameter"/> object to compare with or
    /// <c>null</c>.</param>
    /// <returns><c>true</c> if the content of <paramref name="other"/> is equal to that 
    /// of the current instance.</returns>
    /// <example>
    /// <para>
    /// Comparison of <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample.cs"/>
    /// </example>
    public bool Equals(MimeTypeParameter? other)
        => other is not null && Key.Equals(other.Key, StringComparison.OrdinalIgnoreCase)
           && (IsValueCaseSensitive
                ? StringComparer.Ordinal.Equals(Value, other.Value)
                : StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value));

    /// <summary>
    /// Determines whether <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> object
    /// whose content is equal to that of the current instance.
    /// </summary>
    /// <param name="obj">A <see cref="MimeTypeParameter"/> object to compare with or <c>null</c>.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> object
    /// whose content is equal to that of the current instance.</returns>
    /// <example>
    /// <para>
    /// Comparison of <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample.cs"/>
    /// </example>
    public override bool Equals(object? obj) => obj is MimeTypeParameter parameter && Equals(parameter);
}
