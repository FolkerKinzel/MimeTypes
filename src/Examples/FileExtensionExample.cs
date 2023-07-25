using FolkerKinzel.MimeTypes;

namespace Examples;

public static class FileExtensionExample
{
    public static void Example()
    {
        const string path = @"C:\Users\Tester\Desktop\Interesting Text.odt";

        string mimeType = MimeConverter.FromFileName(path);

        Console.Write($"The MIME type for \"{path}\" is: ");
        Console.WriteLine(mimeType);
        Console.Write("The file type extension for this MIME type is: ");
        Console.WriteLine(MimeConverter.ToFileTypeExtension(mimeType));
    }
}
/*
Console Output:

The MIME type for "C:\Users\Tester\Desktop\Interesting Text.odt" is: application/vnd.oasis.opendocument.text
The file type extension for this MIME type is: .odt
 */
