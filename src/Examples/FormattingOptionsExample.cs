using FolkerKinzel.MimeTypes;

namespace Examples;

public static class FormattingOptionsExample
{
    public static void Example()
    {
        MimeType mime =
            MimeTypeBuilder.Create("application", "x-stuff")
                           .AppendParameter("short", "s")
                           .AppendParameter("key-long",
            "Very very very looong value to demonstrate the line wrapping")
                           .Build();

        Console.WriteLine("FormattingOptions.Default:");
        Console.WriteLine(mime.ToString());
        Console.WriteLine();

        Console.WriteLine("FormattingOptions.None:");
        Console.WriteLine(mime.ToString(FormattingOptions.None));
        Console.WriteLine();

        Console.WriteLine("FormattingOptions.IncludeParameters:");
        Console.WriteLine(mime.ToString(FormattingOptions.IncludeParameters));
        Console.WriteLine();

        Console.WriteLine("FormattingOptions.LineWrapping:");
        Console.WriteLine(mime.ToString(FormattingOptions.LineWrapping));
        Console.WriteLine();

        Console.WriteLine("FormattingOptions.LineWrapping | FormattingOptions.WhiteSpaceBetweenParameters:");
        Console.WriteLine(mime.ToString(FormattingOptions.LineWrapping | FormattingOptions.WhiteSpaceBetweenParameters));
        Console.WriteLine();

        Console.WriteLine("FormattingOptions.AlwaysUrlEncoded:");
        Console.WriteLine(mime.ToString(FormattingOptions.AlwaysUrlEncoded));
        Console.WriteLine();
    }
}

/*
Console Output:

FormattingOptions.Default:
application/x-stuff; short=s; key-long=Very very very looong value to demonstrate the line wrapping

FormattingOptions.None:
application/x-stuff

FormattingOptions.IncludeParameters:
application/x-stuff;short=s;key-long=Very very very looong value to demonstrate the line wrapping

FormattingOptions.LineWrapping:
application/x-stuff;short=s;
key-long*0=Very very very looong value to demonstrate the line ;
key-long*1=wrapping

FormattingOptions.LineWrapping | FormattingOptions.WhiteSpaceBetweenParameters:
application/x-stuff; short=s;
key-long*0=Very very very looong value to demonstrate the line ;
key-long*1=wrapping

FormattingOptions.AlwaysUrlEncoded:
application/x-stuff;short=s;key-long=Very%20very%20very%20looong%20value%20to%20demonstrate%20the%20line%20wrapping
*/