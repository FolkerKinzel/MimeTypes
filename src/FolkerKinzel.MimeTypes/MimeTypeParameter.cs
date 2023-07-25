using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;
using FolkerKinzel.MimeTypes.Properties;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Encapsulates the data of an Internet Media Type parameter.
/// </summary>
public sealed class MimeTypeParameter : IEquatable<MimeTypeParameter>, IComparable<MimeTypeParameter>
{
    /// <summary>
    /// Initializes a new <see cref="MimeTypeParameter"/> instance.
    /// </summary>
    /// <param name="key">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="language">An IETF-Language tag that indicates the language of the parameter's value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="key"/> is <see cref="string.Empty"/>
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="key"/> is not a valid MIME type parameter name
    /// according to RFC 2231,
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="key"/> is longer than 4095 characters
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="language"/> is neither <c>null</c> nor empty nor a valid IETF-Language-Tag according to RFC-1766.
    /// </para>
    /// </exception>
    internal MimeTypeParameter(string key, string? value, string? language = null)
    {
        key.ValidateTokenParameter(nameof(key), true);

        if (key.Length > MimeTypeParameterInfo.KEY_LENGTH_MAX_VALUE)
        {
            throw new ArgumentException(Res.StringTooLong, nameof(key));
        }

        Language = string.IsNullOrEmpty(language) ? null : language;
        ValidateLanguageParameter(Language, nameof(language));

        Key = key.Trim();
        Value = string.IsNullOrEmpty(value) ? null : value;

    }


    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Key { get; }


    /// <summary>
    /// Gets the value of the parameter.
    /// </summary>
    public string? Value { get; }


    /// <summary>
    /// Gets an IETF-Language tag that indicates the language of the parameter's value.
    /// </summary>
    public string? Language { get; }


    private static void ValidateLanguageParameter(string? language, string paraName)
    {
        if (language is null)
        {
            return;
        }

        if (!IetfLanguageTag.Validate(language))
        {
            throw new ArgumentException(string.Format(Res.InvalidIetfLanguageTag, paraName), paraName);
        }
    }

    /// <summary>
    /// Determines if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.
    /// </summary>
    /// <param name="other">A <see cref="MimeTypeParameter"/> object to compare with or <c>null</c>.</param>
    /// <returns><c>true</c> if the content of <paramref name="other"/> is equal to that of the 
    /// current instance.</returns>
    public bool Equals(MimeTypeParameter? other)
        => other is not null && Key.Equals(other.Key, StringComparison.OrdinalIgnoreCase)
           && (MimeTypeParameterInfo.GetIsValueCaseSensitive(Key)
                ? Value.Equals(other.Value, StringComparison.Ordinal)
                : Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase));


    /// <summary>
    /// Determines whether <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> object
    /// whose content is equal to that of the current instance.
    /// </summary>
    /// <param name="obj">A <see cref="MimeTypeParameter"/> object to compare with or <c>null</c>.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> object
    /// whose content is equal to that of the current instance.</returns>
    public override bool Equals(object? obj) => obj is MimeTypeParameter parameter && Equals(parameter);


    /// <summary>
    /// Computes a hash code for the instance.
    /// </summary>
    /// <returns>The hash code for the instance.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Key, StringComparer.OrdinalIgnoreCase);

        if (MimeTypeParameterInfo.GetIsValueCaseSensitive(Key))
        {
            hash.Add(Value, StringComparer.Ordinal);
        }
        else
        {
            hash.Add(Value, StringComparer.OrdinalIgnoreCase);
        }

        return hash.ToHashCode();
    }


    /// <summary>
    /// Compares the current instance with another <see cref="MimeTypeParameter"/> and returns an integer that indicates whether 
    /// the current instance precedes, follows, or occurs in the same position in the sort order as the other <see cref="MimeTypeParameter"/>.
    /// </summary>
    /// <param name="other">The other <see cref="MimeTypeParameter"/> instance to compare with.</param>
    /// <returns>A value that indicates the relative order of the instances being compared.</returns>
    /// <remarks>The method takes only the <see cref="MimeTypeParameter.Key"/>s into account.</remarks>
    public int CompareTo(MimeTypeParameter? other) => StringComparer.OrdinalIgnoreCase.Compare(Key, other?.Key);


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

    /// <summary>
    /// Indicates whether this instance equals "charset=us-ascii". The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance equals "charset=us-ascii"; otherwise, <c>false</c>.</value>
    internal bool IsAsciiCharSetParameter
        => Key.Equals(MimeTypeParameterInfo.CHARSET_KEY, StringComparison.OrdinalIgnoreCase)
           && Value.Equals(MimeTypeParameterInfo.ASCII_CHARSET_VALUE, StringComparison.OrdinalIgnoreCase);
}
