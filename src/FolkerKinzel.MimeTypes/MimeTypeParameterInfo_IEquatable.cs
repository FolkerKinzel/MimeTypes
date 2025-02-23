namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameterInfo : IEquatable<MimeTypeParameterInfo>
{
    /// <summary>
    /// Determines if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.
    /// </summary>
    /// <param name="other">A <see cref="MimeTypeParameterInfo"/> structure to compare with.</param>
    /// <returns><c>true</c> if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.</returns>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MimeTypeParameterInfo other) => Equals(in other);

    /// <summary>
    /// Determines if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.
    /// </summary>
    /// <param name="other">A <see cref="MimeTypeParameterInfo"/> structure to compare with.</param>
    /// <returns><c>true</c> if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.</returns>
    /// <remarks>This is the most performant overload of the Equals methods but unfortunately it's 
    /// not CLS compliant. Use it if you can.</remarks>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    [CLSCompliant(false)]
    public bool Equals(in MimeTypeParameterInfo other)
        => Key.Equals(other.Key, StringComparison.OrdinalIgnoreCase)
           && (IsValueCaseSensitive
                ? Value.Equals(other.Value, StringComparison.Ordinal)
                : Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Determines whether <paramref name="obj"/> is a <see cref="MimeTypeParameterInfo"/> structure
    /// whose content is equal to that of the current instance.
    /// </summary>
    /// <param name="obj">A <see cref="MimeTypeParameterInfo"/> structure to compare with.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeTypeParameterInfo"/> 
    /// structure whose content is equal to that of the current instance.</returns>
    /// <example>
    /// <para>
    /// Efficient parsing of an Internet Media Type <see cref="string"/>:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/MimeTypeInfoExample.cs"/>
    /// </example>
    public override bool Equals(object? obj)
        => obj is MimeTypeParameterInfo parameter && Equals(in parameter);
}
