using System.ComponentModel;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType 
{
    /// <summary>
    /// Obsolete. Use MimeString.OctetStream instead.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use MimeString.OctetStream instead.", false)]
    public const string Default = MimeString.OctetStream;

    /// <summary>
    /// Minimum count of characters at which a line of an Internet Media Type <see cref="string"/> is wrapped.
    /// </summary>
    public const int MinimumLineLength = 64;


    /// <summary>
    /// The estimated length of the <see cref="string"/> that is created when <see cref="ToString()"/> is called.
    /// </summary>
    internal const int STRING_LENGTH = 80;

    internal const string NEW_LINE = "\r\n";


}
