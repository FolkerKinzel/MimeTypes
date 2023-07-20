using FolkerKinzel.MimeTypes.Properties;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class MimeTypeCtorParametersValidator
{
    private const int MEDIA_TYPE_LENGTH_MAX_VALUE = MimeType.MEDIA_TYPE_LENGTH_MAX_VALUE;
    private const int SUB_TYPE_LENGTH_MAX_VALUE = MimeType.SUB_TYPE_LENGTH_MAX_VALUE;


    /// <summary>Validates <paramref name="mediaType"/> and <paramref name="subType"/>.</summary>
    /// <param name="mediaType"></param>
    /// <param name="subType"></param>
    /// <exception cref="ArgumentException">
    /// <paramref name="mediaType"/> or <paramref name="subType"/> is <see cref="string.Empty"/> or is
    /// a <see cref="string"/> that is longer than <see cref="short.MaxValue"/> or contains characters,
    /// which are not permitted by the standard (RFC 2045).
    /// </exception>
    internal static void Validate(string mediaType, string subType)
    {
        Debug.Assert(mediaType != null);
        Debug.Assert(subType != null);

        mediaType.ValidateTokenParameter(nameof(mediaType));
        subType.ValidateTokenParameter(nameof(subType));
        ThrowOnTooLongMediaType(mediaType);
        ThrowOnTooLongSubType(subType);
    }

    private static void ThrowOnTooLongMediaType(string mediaType)
    {
        if (mediaType.Length > MEDIA_TYPE_LENGTH_MAX_VALUE)
        {
            throw new ArgumentException(Res.StringTooLong, nameof(mediaType));
        }
    }

    private static void ThrowOnTooLongSubType(string subType)
    {
        if (subType.Length > SUB_TYPE_LENGTH_MAX_VALUE)
        {
            throw new ArgumentException(Res.StringTooLong, nameof(subType));
        }
    }


}
