namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

internal static class MimeFormatNormalizer
{
    internal static MimeFormat Normalize(this MimeFormat format) => 
        format.HasFlag(MimeFormat.IgnoreParameters)
                 ? MimeFormat.IgnoreParameters
                 : format.HasFlag(MimeFormat.Url)
                        ? MimeFormat.Url
                        : format & (MimeFormat.AvoidSpace | MimeFormat.LineWrapping);
}
