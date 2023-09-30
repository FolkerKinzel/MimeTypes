using System.ComponentModel;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType 
{
    /// <summary>
    /// Obsolete. Use MimeString.OctetStream instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use MimeString.OctetStream instead.", true)]
    [Browsable(false)]
    public const string Default = MimeString.OctetStream;


    /// <summary>
    /// Minimum count of characters at which a line of an Internet Media Type <see cref="string"/> is wrapped.
    /// </summary>
    public const int MinLineLength = 64;


    internal const string NEW_LINE = "\r\n";


}
