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
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public bool IsAsciiCharSetParameter
        => IsCharSetParameter
           && StringComparer.OrdinalIgnoreCase.Equals(Value, MimeTypeParameter.ASCII_CHARSET_VALUE);


    /// <summary>
    /// Indicates whether the <see cref="MimeTypeParameterInfo"/> has the <see cref="Key"/> "charset". The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if <see cref="Key"/> equals "charset"; otherwise, <c>false</c>.</value>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public bool IsCharSetParameter
        => Key.Equals(MimeTypeParameter.CHARSET_KEY, StringComparison.OrdinalIgnoreCase);


    /// <summary>
    /// Indicates whether the <see cref="MimeTypeParameterInfo"/> has the <see cref="Key"/> "access-type". The comparison is case-insensitive.
    /// </summary>
    /// <value><c>true</c> if <see cref="Key"/> equals "access-type"; otherwise, <c>false</c>.</value>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public bool IsAccessTypeParameter => Key.Equals(MimeTypeParameter.ACCESS_TYPE_KEY, StringComparison.OrdinalIgnoreCase);


    /// <summary>
    /// Indicates whether the <see cref="Value"/> should be treated case sensitive.
    /// </summary>
    /// <example>
    /// <para>
    /// Building, serializing, parsing, and editing of  <see cref="MimeType"/> instances:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
    /// </example>
    public bool IsValueCaseSensitive => !(IsCharSetParameter || IsAccessTypeParameter);
    // Change MimeTypeParameterInfo.IsValueCaseSensitive when this property is edited!
}
