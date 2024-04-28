namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

internal static class MimeFormatsNormalizer
{
    internal static MimeFormats Normalize(this MimeFormats format) =>
        format.HasFlag(MimeFormats.IgnoreParameters)
                 ? MimeFormats.IgnoreParameters
                 : format.HasFlag(MimeFormats.Url)
                        ? MimeFormats.Url
                        : format & (MimeFormats.AvoidSpace | MimeFormats.LineWrapping);
}
