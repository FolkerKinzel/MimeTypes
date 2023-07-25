using FolkerKinzel.MimeTypes;

namespace Examples;

public static class FileExtensionExample
{
    public static void Example()
    {
        const string path = @"C:\Users\Tester\Desktop\Interesting Text.odt";

        string extension = Path.GetExtension(path);
        string mimeType = MimeType.FromFileExtension(extension);

        Console.Write($"The MIME type for \"{extension}\" is: ");
        Console.WriteLine(mimeType);
        Console.Write("The file type extension for this MIME type is: ");
        Console.WriteLine(MimeType.ToFileExtension(mimeType));
    }
}
/*
Console Output:

The MIME type for ".odt" is: application/vnd.oasis.opendocument.text
The file type extension for this MIME type is: .odt
 */
