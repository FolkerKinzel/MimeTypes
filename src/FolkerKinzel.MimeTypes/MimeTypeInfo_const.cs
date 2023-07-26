using System.ComponentModel;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeInfo
{

    internal const int MEDIA_TYPE_LENGTH_MAX_VALUE = short.MaxValue;
    internal const int SUB_TYPE_LENGTH_MAX_VALUE = short.MaxValue;

    private const int SUB_TYPE_LENGTH_SHIFT = 1;
    private const int MEDIA_TYPE_LENGTH_SHIFT = 16;

    private const string NEW_LINE = "\r\n";

    /// <summary>
    /// The estimated length of the <see cref="string"/> that is created when <see cref="ToString()"/> is called.
    /// </summary>
    private const int STRING_LENGTH = 80;


    
}
