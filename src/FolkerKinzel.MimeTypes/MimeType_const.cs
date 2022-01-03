using System.ComponentModel;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
{
    #region const

    private const string NEW_LINE = "\r\n";

    /// <summary>
    /// The estimated length of the <see cref="string"/> that is created when <see cref="ToString()"/> is called.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const int StringLength = 80;


    /// <summary>
    /// Minimum count of characters at which a line is wrapped.
    /// </summary>
    public const int MinimumLineLength = 64;



    #endregion
}
