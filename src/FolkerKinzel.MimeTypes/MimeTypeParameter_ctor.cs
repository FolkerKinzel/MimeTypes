using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Encapsulates a parameter of a <see cref="MimeType"/>.
/// </summary>
/// <remarks>
/// <note type="tip">
/// <para>
/// <see cref="MimeTypeParameter"/> is a quite large structure. Pass it to other methods by reference (in, ref or out parameters in C#)!
/// </para>
/// <para>
/// If you intend to hold a <see cref="MimeTypeParameter"/> for a long time in memory and if this <see cref="MimeTypeParameter"/> is parsed
/// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
/// keep in mind, that the <see cref="MimeTypeParameter"/> holds a reference to that <see cref="string"/>. Consider in this case to make
/// a copy of the <see cref="MimeType"/> structure with <see cref="MimeTypeParameter.Clone"/>: The copy is built on a separate <see cref="string"/>,
/// which is case-normalized and only as long as needed.
/// </para>
/// </note>
/// </remarks>
[StructLayout(LayoutKind.Auto)]
public readonly partial struct MimeTypeParameter
{
    /// <summary>
    /// Initializes a new <see cref="MimeTypeParameter"/> structure.
    /// </summary>
    /// <param name="parameterString">The trimmed Parameter.</param>
    /// <param name="idx">An Int32, which stores the indexes.</param>
    private MimeTypeParameter(in ReadOnlyMemory<char> parameterString, int idx)
    {
        this._parameterString = parameterString;
        this._idx = idx;
    }



}
