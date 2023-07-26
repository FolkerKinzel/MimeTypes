namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeTypeParameter
{
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


    /// <summary>
    /// Indicates whether this instance equals "charset=us-ascii". The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance equals "charset=us-ascii"; otherwise, <c>false</c>.</value>
    internal bool IsAsciiCharSetParameter
        => Key.Equals(MimeTypeParameterInfo.CHARSET_KEY, StringComparison.OrdinalIgnoreCase)
           && StringComparer.OrdinalIgnoreCase.Equals(Value, MimeTypeParameterInfo.ASCII_CHARSET_VALUE);
}
