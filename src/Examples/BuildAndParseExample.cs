using FolkerKinzel.MimeTypes;

namespace Examples;

public static class BuildAndParseExample
{
    public static void Example()
    {
        const string longParameterValue = """
        This is a very long parameter that will be wrapped according to RFC 2184.
        It also contains a few Non-ASCII-Characters: äöß.
        """;

        MimeTypeInfo mimeType1 = 
            MimeTypeBuilder.Create("application", "x-stuff")
                           .AppendParameter("first-parameter", longParameterValue, "en")
                           .AppendParameter("second-parameter", "Parameter with  \\, = and \".")
                           .Build();

        string s = mimeType1.ToString(MimeFormats.LineWrapping);
        Console.WriteLine(s);

        var mimeType2 = MimeTypeInfo.Parse(s);

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

application/x-stuff;
first-parameter*0*=utf-8'en'This%20is%20a%20very%20long%20param;
first-parameter*1*=eter%20that%20will%20be%20wrapped%20accordin;
first-parameter*2*=g%20to%20RFC%202184.%0D%0AIt%20also%20contai;
first-parameter*3*=ns%20a%20few%20Non-ASCII-Characters%3A%20%C3;
first-parameter*4*=%A4%C3%B6%C3%9F.;
second-parameter="Parameter with  \\, = and \"."

Media Type: application
Sub Type:   x-stuff

Parameter 1:
============
Key:      first-parameter
Language: en
Value:
This is a very long parameter that will be wrapped according to RFC 2184.
It also contains a few Non-ASCII-Characters: äöß.

Parameter 2:
============
Key:      second-parameter
Language:
Value:
Parameter with  \, = and ".
 */
