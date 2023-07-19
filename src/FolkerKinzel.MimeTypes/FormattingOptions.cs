namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Options for formatting a <see cref="MimeType"/> as <see cref="string"/>. The flags can be combined.
/// </summary>
[Flags]
public enum FormattingOptions
{
    /// <summary>
    /// No options are selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// Select this option, to have the <see cref="MimeType.Parameters"/> in the serialized <see cref="string"/>.
    /// </summary>
    IncludeParameters = 1,

    /// <summary>
    /// If selected, one Space character is inserted before each parameter.
    /// </summary>
    WhiteSpaceBetweenParameters = 1 << 1,

    /// <summary>
    /// If selected, the output is wrapped if its length exceeds a given maximum. Parameters are
    /// splittet according to RFC 2184.
    /// </summary>
    LineWrapping = 1 << 2,

    /// <summary>
    /// The default setting. (It is equivalent to <see cref="IncludeParameters"/> | <see cref="WhiteSpaceBetweenParameters"/>).
    /// </summary>
    Default = WhiteSpaceBetweenParameters | IncludeParameters,

    /// <summary>
    /// If selected, parameter values are always URL encoded and the options <see cref="LineWrapping"/> and
    /// <see cref="WhiteSpaceBetweenParameters"/> are ignored. (Without this option, URL encoding is used only
    /// for Non-ASCII characters.)
    /// </summary>
    AlwaysUrlEncoded = 1 << 8

}
