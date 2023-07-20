using FolkerKinzel.MimeTypes;

namespace Examples;

public static class BuildAndParseExample
{
    public static void Example()
    {
        const string longParameterValue = """
        This is a very long parameter, which will be wrapped according to RFC 2184.
        It contains also a few Non-ASCII-Characters: äöß.
        """;

        MimeType mimeType1 = 
            MimeTypeBuilder.Create("application", "x-stuff")
                           .AppendParameter("first-parameter", longParameterValue, "en")
                           .AppendParameter("second-parameter", "Parameter with  \\, = and \".")
                           .Build();

        string s = mimeType1.ToString(FormattingOptions.LineWrapping | FormattingOptions.Default);
        Console.WriteLine(s);

        var mimeType2 = MimeType.Parse(s);

        Console.WriteLine();
        Console.WriteLine($"Media Type: {mimeType2.MediaType.ToString()}");
        Console.WriteLine($"Sub Type:   {mimeType2.SubType.ToString()}");

        int parameterCounter = 1;
        foreach (MimeTypeParameter parameter in mimeType2.Parameters())
        {
            Console.WriteLine();
            Console.WriteLine($"Parameter {parameterCounter++}:");
            Console.WriteLine("============");
            Console.WriteLine($"Key:      {parameter.Key}");
            Console.WriteLine($"Language: {parameter.Language}");
            Console.WriteLine("Value:");
            Console.WriteLine(parameter.Value.ToString());
        }
    }
}
/*
Console Output:

first-parameter*0*=utf-8'en'This%20is%20a%20very%20long%20param;
first-parameter*1*=eter%2C%20which%20will%20be%20wrapped%20acco;
first-parameter*2*=rding%20to%20RFC%202184.%0D%0AIt%20contains%;
first-parameter*3*=20also%20a%20few%20Non-ASCII-Characters%3A%2;
first-parameter*4*=0%C3%A4%C3%B6%C3%9F.;
second-parameter="Parameter with  \\, = and \"."

Media Type: application
Sub Type:   x-stuff

Parameter 1:
============
Key:      first-parameter
Language: en
Value:
This is a very long parameter, which will be wrapped according to RFC 2184.
It contains also a few Non-ASCII-Characters: äöß.

Parameter 2:
============
Key:      second-parameter
Language:
Value:
Parameter with  \, = and ".
 */
