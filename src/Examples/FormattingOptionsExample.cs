using FolkerKinzel.MimeTypes;

namespace Examples;

public static class FormattingOptionsExample
{
    public static void Example()
    {
        MimeType mimeType =
            MimeType.Create("application", "x-stuff")
                           .AppendParameter("short", "s")
                           .AppendParameter("key-long",
            "Very very loooong value in order to show the line wrapping");

        Console.WriteLine("MimeFormats.Default:");
        Console.WriteLine(mimeType.ToString());
        Console.WriteLine();

        Console.WriteLine("MimeFormats.IgnoreParameters:");
        Console.WriteLine(mimeType.ToString(MimeFormats.IgnoreParameters));
        Console.WriteLine();

        Console.WriteLine("MimeFormats.AvoidSpace:");
        Console.WriteLine(mimeType.ToString(MimeFormats.AvoidSpace));
        Console.WriteLine();

        Console.WriteLine("MimeFormats.LineWrapping:");
        Console.WriteLine(mimeType.ToString(MimeFormats.LineWrapping));
        Console.WriteLine();

        Console.WriteLine("MimeFormats.LineWrapping | MimeFormats.AvoidSpace:");
        Console.WriteLine(mimeType.ToString(MimeFormats.LineWrapping | MimeFormats.AvoidSpace));
        Console.WriteLine();

        Console.WriteLine("MimeFormats.Url:");
        Console.WriteLine(mimeType.ToString(MimeFormats.Url));
        Console.WriteLine();
    }
}

/*
Console Output:

MimeFormats.Default:
application/x-stuff; short=s; key-long="Very very loooong value in order to show the line wrapping"

MimeFormats.IgnoreParameters:
application/x-stuff

MimeFormats.AvoidSpace:
application/x-stuff;short=s;key-long="Very very loooong value in order to show the line wrapping"

MimeFormats.LineWrapping:
application/x-stuff; short=s;
key-long*0="Very very loooong value in order to show the line ";
key-long*1="wrapping"

MimeFormats.LineWrapping | MimeFormats.AvoidSpace:
application/x-stuff;short=s;
key-long*0="Very very loooong value in order to show the line ";
key-long*1="wrapping"

MimeFormats.Url:
application/x-stuff;short=s;key-long*=utf-8''Very%20very%20loooong%20value%20in%20order%20to%20show%20the%20line%20wrapping
*/