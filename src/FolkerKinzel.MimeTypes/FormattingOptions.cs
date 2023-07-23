﻿namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Options for formatting a <see cref="MimeType"/> as <see cref="string"/>. The flags can be combined.
/// </summary>
/// <example>
/// <para>Format a <see cref="MimeType"/> instance into a standards-compliant string using several options:</para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FormattingOptionsExample.cs"/>
/// </example>
[Flags]
[SuppressMessage("Design", "CA1069:Enumerationswerte dürfen nicht dupliziert werden", Justification = "<Ausstehend>")]
public enum FormattingOptions
{
    /// <summary>
    /// No options are selected. If only this option is selected, parameters are not included in the 
    /// formatted output.
    /// </summary>
    None = 0,

    /// <summary>
    /// Select this option, to have the <see cref="MimeType.Parameters"/> in the serialized <see cref="string"/>.
    /// </summary>
    IncludeParameters = 1,

    /// <summary>
    /// If selected, one Space character is inserted before each parameter. This option is combined with
    /// <see cref="IncludeParameters"/>.
    /// </summary>
    WhiteSpaceBetweenParameters = (1 << 1) | IncludeParameters,

    /// <summary>
    /// If selected, the output is wrapped if its length exceeds a given maximum. Parameters are
    /// splittet according to RFC 2231. This option is combined with
    /// <see cref="IncludeParameters"/>.
    /// </summary>
    LineWrapping = (1 << 2) | IncludeParameters,

    /// <summary>
    /// The default setting. (It is equivalent to <see cref="IncludeParameters"/> | <see cref="WhiteSpaceBetweenParameters"/>).
    /// </summary>
    Default = WhiteSpaceBetweenParameters | IncludeParameters,

    /// <summary>
    /// <para>
    /// If selected, parameter values are URL-encoded even if they didn't contain non-ASCII characters. Choosing this 
    /// option, the options <see cref="LineWrapping"/> and
    /// <see cref="WhiteSpaceBetweenParameters"/> are ignored.
    /// </para>
    /// <para>
    /// This option is combined with <see cref="IncludeParameters"/>.
    /// </para>
    /// </summary>
    AlwaysUrlEncoded = (1 << 8) | IncludeParameters,

}
