namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Options for formatting a <see cref="MimeType"/> as <see cref="string"/>. The flags can be combined.
/// </summary>
/// <example>
/// <para>Format a <see cref="MimeType"/> instance into a standards-compliant string using several options:</para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FormattingOptionsExample.cs"/>
/// </example>
[Flags]
public enum MimeFormat
{
    /// <summary>
    /// Specifies the default format (including the <see cref="MimeType.Parameters"/>, without 
    /// line wrapping, and with one <c>SPACE</c> character (U+0020) before each parameter).
    /// </summary>
    Default = 0,

    /// <summary>
    /// Specifies that the <see cref="MimeType.Parameters"/> are not included in the output and
    /// any other option is ignored.
    /// </summary>
    IgnoreParameters = 1,

    /// <summary>
    /// Specifies that no white space will be added before each parameter.
    /// </summary>
    AvoidSpace = 2,

    /// <summary>
    /// Specifies that the output is formatted for the use within a URI. This option is combined 
    /// with <see cref="AvoidSpace"/>.
    /// </summary>
    Url = 6,

    /// <summary>
    /// Specifies that long parameters will be wrapped according to RFC 2231 if its 
    /// length exceeds a given maximum. This option is ignored if any of the flags
    /// <see cref="IgnoreParameters"/> or <see cref="Url"/> is set.
    /// </summary>
    LineWrapping = 8
}
