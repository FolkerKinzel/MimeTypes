using FolkerKinzel.MimeTypes.Intls;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType : IEquatable<MimeType>
{
    /// <summary>
    /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. The <see cref="Parameters"/>
    /// are taken into account.
    /// </summary>
    /// <param name="other">The <see cref="MimeType"/> instance to compare with or <c>null</c>.</param>
    /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
    /// 
    /// <example>
    /// <para>
    /// Comparing <see cref="MimeType"/> instances for equality:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample2.cs"/>
    /// </example>
    [CLSCompliant(false)]
    public bool Equals(MimeType? other) => Equals(other, false);


    /// <summary>
    /// Determines whether this instance is equal to <paramref name="other"/> and allows to specify
    /// whether or not the <see cref="Parameters"/> are taken into account.
    /// </summary>
    /// <param name="other">The <see cref="MimeType"/> instance to compare with or <c>null</c>.</param>
    /// <param name="ignoreParameters">Pass <c>false</c> to take the <see cref="Parameters"/> into account;
    /// <c>true</c>, otherwise.</param>
    /// <returns><c>true</c> if this  instance is equal to <paramref name="other"/>; false, otherwise.</returns>
    /// <example>
    /// <para>
    /// Comparing <see cref="MimeType"/> instances for equality:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample2.cs"/>
    /// </example>
    public bool Equals(MimeType? other, bool ignoreParameters)
    {
        if (other == null)
        {
            return false;
        }
        if (!MediaType.Equals(other.MediaType, StringComparison.OrdinalIgnoreCase) ||
           !SubType.Equals(other.SubType, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (ignoreParameters)
        {
            return true;
        }

        bool isText = IsText;
        return this.Parameters.Sort(isText).SequenceEqual(other.Parameters.Sort(isText));
    }


    /// <summary>
    /// Determines whether <paramref name="obj"/> is a <see cref="MimeType"/> object
    /// whose
    /// value is equal to that of this instance. The <see cref="Parameters"/>
    /// are taken into account.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare with or <c>null</c>.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeType"/> object whose
    /// value is equal to that of this instance; <c>false</c>, otherwise.</returns>
    /// <example>
    /// <para>
    /// Comparing <see cref="MimeType"/> instances for equality:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/EqualityExample2.cs"/>
    /// </example>
    public override bool Equals(object? obj) => obj is MimeType type && Equals(type, false);

}
