namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType 
{
    /// <summary>
    /// The default Internet Media Type ("MIME type").
    /// </summary>
    public const string Default = "application/octet-stream";

    /// <summary>
    /// Minimum count of characters at which a line of an Internet Media Type <see cref="string"/> is wrapped.
    /// </summary>
    public const int MinimumLineLength = 64;


    /// <summary>
    /// The estimated length of the <see cref="string"/> that is created when <see cref="ToString()"/> is called.
    /// </summary>
    internal const int STRING_LENGTH = 80;

    private const string NEW_LINE = "\r\n";


}
