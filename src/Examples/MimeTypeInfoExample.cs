using FolkerKinzel.MimeTypes;

namespace Examples;

public static class MimeTypeInfoExample
{
    public static void Example()
    {
        const string input = """
        This is some text before the MIME type.
                             text/plain; charset=iso-8859-1; (This is a comment.)
        second-parameter*0*=utf-8'en'For%20demonstration%20purposes%20o;
        second-parameter*1*=nly%2C%20with%20a%20few%20non-ASCII%20chara;
        second-parameter*2*=cters%20%C3%A4%C3%B6%C3%BC
        
                        This is some Text after the MIME type.
        """;

        // MimeTypeInfo parses a specified part of a longer string
        // without having to allocate a substring.
        // White space characters before and after the MIME type are accepted.
        // The return values of the properties are portions of the input in form
        // of ReadOnlySpan<char> structs.

        MimeTypeInfo info = MimeTypeInfo.Parse(input.AsMemory(39, 275));

        Console.WriteLine("Media Type: {0}", info.MediaType.ToString());
        Console.WriteLine("Sub Type:   {0}", info.SubType.ToString());
        Console.WriteLine();
        Console.WriteLine("Is empty:        {0}", info.IsEmpty);
        Console.WriteLine("Is text:         {0}", info.IsText);
        Console.WriteLine("Is plain text:   {0}", info.IsTextPlain);
        Console.WriteLine("Is octet stream: {0}", info.IsOctetStream);

        Console.WriteLine("The file type extension for this MIME type is \"{0}\".",
                           info.GetFileTypeExtension());

        int parameterCounter = 1;
        foreach (MimeTypeParameterInfo parameter in info.Parameters())
        {
            Console.WriteLine();
            Console.WriteLine($"Parameter {parameterCounter++}:");
            Console.WriteLine("============");
            Console.WriteLine($"Key:       {parameter.Key}");
            Console.WriteLine($"Value:     {parameter.Value}");
            Console.WriteLine($"Language:  {parameter.Language}");
            Console.WriteLine($"Charset:   {parameter.CharSet}");
            Console.WriteLine("Is charset parameter:       {0}", parameter.IsCharSetParameter);
            Console.WriteLine("Is ASCII charset parameter: {0}", parameter.IsAsciiCharSetParameter);
            Console.WriteLine("Is access type parameter:   {0}", parameter.IsAccessTypeParameter);
            Console.WriteLine("Is value case sensitive:    {0}", parameter.IsValueCaseSensitive);
        }
        Console.WriteLine();

        // Compare MimeTypeInfo values using options:
        MimeTypeInfo info2 = MimeTypeInfo.Parse("TEXT/PLAIN; CHARSET=UTF-8");
        Console.WriteLine("Equal with parameters:      {0}", info.Equals(in info2));
        Console.WriteLine("Equal without parameters:   {0}", info.Equals(in info2, ignoreParameters: true));
    }
}
/*
Console output: 

Media Type: text
Sub Type:   plain

Is empty:        False
Is text:         True
Is plain text:   True
Is octet stream: False
The file type extension for this MIME type is ".txt".

Parameter 1:
============
Key:       charset
Value:     iso-8859-1
Language:
Charset:
Is charset parameter:       True
Is ASCII charset parameter: False
Is access type parameter:   False
Is value case sensitive:    False

Parameter 2:
============
Key:       second-parameter
Value:     For demonstration purposes only, with a few non-ASCII characters äöü
Language:  en
Charset:   utf-8
Is charset parameter:       False
Is ASCII charset parameter: False
Is access type parameter:   False
Is value case sensitive:    True

Equal with parameters:      False
Equal without parameters:   True
 */
