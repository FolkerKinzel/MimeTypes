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

}
