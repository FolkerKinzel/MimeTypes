using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Provides the information stored in an Internet Media Type parameter-string.
/// </summary>
/// <remarks>
/// <note type="tip">
/// <para>
/// <see cref="MimeTypeParameterInfo"/> is a quite large structure. Pass it to other methods by reference (in, ref or out parameters in C#)!
/// </para>
/// <para>
/// If you intend to hold a <see cref="MimeTypeParameterInfo"/> for a long time in memory and if this <see cref="MimeTypeParameterInfo"/> is parsed
/// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
/// keep in mind, that the <see cref="MimeTypeParameterInfo"/> holds a reference to that <see cref="string"/>. Consider in this case to make
/// a copy of the <see cref="MimeTypeInfo"/> structure with <see cref="MimeTypeParameterInfo.Clone"/>: The copy is built on a separate <see cref="string"/>,
/// which is case-normalized and only as long as needed.
/// </para>
/// </note>
/// </remarks>
/// <example>
/// <para>
/// Build, serialize, and parse a <see cref="MimeTypeInfo"/> instance:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/BuildAndParseExample.cs"/>
/// <para>Format a <see cref="MimeTypeInfo"/> instance into a standards-compliant string using several options:</para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FormattingOptionsExample.cs"/>
/// </example>
[StructLayout(LayoutKind.Auto)]
public readonly partial struct MimeTypeParameterInfo
{
    /// <summary>
    /// Initializes a new <see cref="MimeTypeParameterInfo"/> structure.
    /// </summary>
    /// <param name="parameterString">The trimmed Parameter.</param>
    /// <param name="idx">An Int32, which stores the indexes.</param>
    private MimeTypeParameterInfo(in ReadOnlyMemory<char> parameterString, int idx)
    {
        this._parameterString = parameterString;
        this._idx = idx;
    }



}
