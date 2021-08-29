using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;

namespace Examples
{
    public static class FileExtensionExample
    {
        public static void Example()
        {
            const string path = @"C:\Users\Tester\Desktop\Interesting Text.odt";

            string extension = Path.GetExtension(path);
            MimeType mimeType = MimeType.FromFileTypeExtension(extension);

            Console.Write($"The MIME type for \"{extension}\" is: ");
            Console.WriteLine(mimeType);
            Console.Write("The file type extension for this MIME type is: ");
            Console.WriteLine(mimeType.GetFileTypeExtension());
        }
    }
}

/*
Console Output:

The MIME type for ".odt" is: application/vnd.oasis.opendocument.text
The file type extension for this MIME type is: .odt
.
 */
