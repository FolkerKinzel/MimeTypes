using FolkerKinzel.MimeTypes;

namespace Examples;

public static class BuildAndParseExample
{
    public static void Example()
    {
        var dic = new ParameterModelDictionary()
            {
                new ParameterModel("first-parameter",
                "This is a very long parameter, which will be wrapped according to RFC 2184." +
                Environment.NewLine +
                "It contains also a few Non-ASCII-Characters: \u00E4\u00D6\u00DF.", "en"),
                new ParameterModel("second-parameter", "Parameter with  \\, = and \".")
            };

        var mimeType1 = new MimeType("application", "x-stuff", dic);
        string s = mimeType1.ToString(MimeTypeFormattingOptions.LineWrapping | MimeTypeFormattingOptions.Default);
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
            Console.WriteLine($"Key:      {parameter.Key.ToString()}");
            Console.WriteLine($"Value:    {parameter.Value.ToString()}");
            Console.WriteLine($"Language: {parameter.Language.ToString()}");
        }
    }
}
/*
Console Output:

application/x-stuff;
first-parameter*1*=utf-8'en'This%20is%20a%20very%20long%20param;
first-parameter*2*=eter%2C%20which%20will%20be%20wrapped%20acco;
first-parameter*3*=rding%20to%20RFC%202184.%0D%0AIt%20contains;
first-parameter*4*=%20also%20a%20few%20Non-ASCII-Characters%3A;
first-parameter*5*=%20%C3%A4%C3%96%C3%9F.;
second-parameter="Parameter with  \\, = and \"."

Media Type: application
Sub Type:   x-stuff

Parameter 1:
============
Key:      first-parameter
Value:    This is a very long parameter, which will be wrapped according to RFC 2184.
It contains also a few Non-ASCII-Characters: äÖß.
Language: en

Parameter 2:
============
Key:      second-parameter
Value:    Parameter with  \, = and ".
Language:
 */
