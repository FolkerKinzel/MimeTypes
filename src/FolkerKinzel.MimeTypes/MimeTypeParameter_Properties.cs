namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeTypeParameter
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public string Key { get; }


    /// <summary>
    /// Gets the value of the parameter.
    /// </summary>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public string? Value { get; }


    /// <summary>
    /// Gets an IETF-Language tag that indicates the language of the parameter's value.
    /// </summary>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public string? Language { get; }


    /// <summary>
    /// Indicates whether this instance equals "charset=us-ascii". The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if this instance equals "charset=us-ascii"; otherwise, <c>false</c>.</value>
    internal bool IsAsciiCharSetParameter
        => Key.Equals(MimeTypeParameter.CHARSET_KEY, StringComparison.OrdinalIgnoreCase)
           && StringComparer.OrdinalIgnoreCase.Equals(Value, MimeTypeParameter.ASCII_CHARSET_VALUE);
}
