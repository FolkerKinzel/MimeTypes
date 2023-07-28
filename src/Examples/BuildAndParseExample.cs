using FolkerKinzel.MimeTypes;

namespace Examples;

public static class BuildAndParseExample
{
    public static void Example()
    {
        const string longParameterValue = """
        This is a very long parameter that will be wrapped according to RFC 2231.
        It also contains a few Non-ASCII-Characters: äöß.
        """;

        MimeType mimeType1 =
            MimeType.Create("application", "x-stuff")
                    .AppendParameter("first-parameter", longParameterValue, "en")
                    .AppendParameter("second-parameter", "Parameter with  \\, = and \".");

        string s = mimeType1.ToString(MimeFormats.LineWrapping);
        Console.WriteLine(s);

        var mimeType2 = MimeType.Parse(s);

        Console.WriteLine();
        Console.WriteLine("Media Type: {0}", mimeType2.MediaType);
        Console.WriteLine("Sub Type:   {0}", mimeType2.SubType);
        Console.WriteLine();
        Console.WriteLine("Is text:         {0}", mimeType2.IsText);
        Console.WriteLine("Is plain text:   {0}", mimeType2.IsTextPlain);
        Console.WriteLine("Is octet stream: {0}", mimeType2.IsOctetStream);

        int parameterCounter = 1;
        foreach (MimeTypeParameter parameter in mimeType2.Parameters)
        {
            Console.WriteLine();
            Console.WriteLine($"Parameter {parameterCounter++}:");
            Console.WriteLine("============");
            Console.WriteLine($"Key:      {parameter.Key}");
            Console.WriteLine($"Language: {parameter.Language}");
            Console.WriteLine("Value:");
            Console.WriteLine(parameter.Value);
            Console.WriteLine();
            Console.WriteLine("Is charset parameter:       {0}", parameter.IsCharSetParameter);
            Console.WriteLine("Is ASCII charset parameter: {0}", parameter.IsAsciiCharSetParameter);
            Console.WriteLine("Is access type parameter:   {0}", parameter.IsAccessTypeParameter);
            Console.WriteLine("Is value case sensitive:    {0}", parameter.IsValueCaseSensitive);
        }

        // The MimeType class allows to modify the parameters:
        mimeType2.RemoveParameter("first-parameter")
                 .AppendParameter("Second-Parameter", "normal");

        Console.WriteLine();
        Console.Write("mimeType2 modified: ");
        Console.WriteLine(mimeType2);
    }
}
/*
Console Output:

application/x-stuff;
first-parameter*0*=utf-8'en'This%20is%20a%20very%20long%20param;
first-parameter*1*=eter%20that%20will%20be%20wrapped%20accordin;
first-parameter*2*=g%20to%20RFC%202231.%0D%0AIt%20also%20contai;
first-parameter*3*=ns%20a%20few%20Non-ASCII-Characters%3A%20%C3;
first-parameter*4*=%A4%C3%B6%C3%9F.;
second-parameter="Parameter with  \\, = and \"."

Media Type: application
Sub Type:   x-stuff

Is text:         False
Is plain text:   False
Is octet stream: False

Parameter 1:
============
Key:      first-parameter
Language: en
Value:
This is a very long parameter that will be wrapped according to RFC 2231.
It also contains a few Non-ASCII-Characters: äöß.

Is charset parameter:       False
Is ASCII charset parameter: False
Is access type parameter:   False
Is value case sensitive:    True

Parameter 2:
============
Key:      second-parameter
Language:
Value:
Parameter with  \, = and ".

Is charset parameter:       False
Is ASCII charset parameter: False
Is access type parameter:   False
Is value case sensitive:    True

mimeType2 modified: application/x-stuff; second-parameter=normal
 */
