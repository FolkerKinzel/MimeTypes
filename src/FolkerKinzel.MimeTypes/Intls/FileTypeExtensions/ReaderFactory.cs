using FolkerKinzel.MimeTypes.Properties;
using System.Reflection;

namespace FolkerKinzel.MimeTypes.Intls.FileTypeExtensions;

internal static class ReaderFactory
{
    private const string MIME_FILE_RESOURCE_NAME = "FolkerKinzel.MimeTypes.Resources.Mime.csv";
    private const string EXTENSION_FILE_RESOURCE_NAME = "FolkerKinzel.MimeTypes.Resources.Extension.csv";
    private const string INDEX_FILE_RESOURCE_NAME = "FolkerKinzel.MimeTypes.Resources.MimeIdx.csv";

    internal static StreamReader InitMimeFileReader() => InitReader(MIME_FILE_RESOURCE_NAME);
    internal static StreamReader InitExtensionFileReader() => InitReader(EXTENSION_FILE_RESOURCE_NAME);
    internal static StreamReader InitIndexFileReader() => InitReader(INDEX_FILE_RESOURCE_NAME);

    [ExcludeFromCodeCoverage]
    private static StreamReader InitReader(string resourcePath)
    {
        Stream? stream;

        // GetManifestResourceStream may throw many different Exceptions but it seems this
        // could only happen at development time. - So I decided not to catch these Exceptions
        // and let the application crash when debugging to fix potential problems forever.
        stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);

#if DEBUG
        return stream is null
            ? throw new InvalidDataException(string.Format("The resource {0} was not found.", resourcePath))
            : new StreamReader(stream);
#else
            return new StreamReader(stream!);
#endif
    }
}
