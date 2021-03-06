namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameter : IEquatable<MimeTypeParameter>
{
    /// <summary>
    /// Determines if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.
    /// </summary>
    /// <param name="other">A <see cref="MimeTypeParameter"/> structure to compare with.</param>
    /// <returns><c>true</c> if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MimeTypeParameter other) => Equals(in other);


    /// <summary>
    /// Determines if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.
    /// </summary>
    /// <param name="other">A <see cref="MimeTypeParameter"/> structure to compare with.</param>
    /// <returns><c>true</c> if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.</returns>
    /// <remarks>This is the most performant overload of the Equals methods but unfortunately it's not CLS compliant.
    /// Use it if you can.</remarks>
    [CLSCompliant(false)]
    public bool Equals(in MimeTypeParameter other)
        => Key.Equals(other.Key, StringComparison.OrdinalIgnoreCase)
           && (IsValueCaseSensitive
                ? Value.Equals(other.Value, StringComparison.Ordinal)
                : Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase));


    /// <summary>
    /// Determines whether <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> structure
    /// whose content is equal to that of the current instance.
    /// </summary>
    /// <param name="obj">A <see cref="MimeTypeParameter"/> structure to compare with.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> structure
    /// whose content is equal to that of the current instance.</returns>
    public override bool Equals(object? obj) => obj is MimeTypeParameter parameter && Equals(in parameter);

}
