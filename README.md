# FolkerKinzel.MimeTypes
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.MimeTypes)](https://www.nuget.org/packages/FolkerKinzel.MimeTypes/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)

#### .NET library that supports the work with Internet Media Types ("MIME Types").
[Project Reference and Release Notes](https://github.com/FolkerKinzel/MimeTypes/releases/tag/v2.0.0-beta.1)

The library contains:
- `readonly struct MimeType`: Represents a MIME type ("Internet Media Type") according to RFC 2045, RFC 2046 and RFC 2184. The struct can be created from a file type extension or parsed from an Internet Media Type string or `ReadOnlyMemory<Char>`.
- The `MimeType` struct is able 
  - to retrieve an appropriate file type extension for its content automatically,
  - to serialize its content as an Internet Media Type string according to the standards,
  - to compare its content with other Internet Media Types for equality.
- The class `MimeTypeBuilder` offers a fluent API to build `MimeType` instances from scratch.
- The `FormattingOptions` enum allows a featured string serialization of `MimeType` instances.

The library is designed to support performance and small heap allocation. To have a a detailed explanation where the data for the file type extension parser comes from and how it is compiled, have a look at [MimeResourceCompiler](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/MimeResourceCompiler/Program.cs) 
that is part of this repository.

[Version History](https://github.com/FolkerKinzel/MimeTypes/releases)

.

### Examples
1. [Get a `MimeType` instance from a file type extension and vice versa](#example1)
2. [Build, serialize, and parse a `MimeType` instance](#example2)
3. [Format a `MimeType` instance into a standards-compliant string using several options](#example3)
4. [Compare `MimeType` instances](#example4)

#### <a name="example1">1.</a> Get a `MimeType` instance from a file type extension and vice versa:
```csharp
using FolkerKinzel.MimeTypes;

namespace Examples;

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
/*
Console Output:

The MIME type for ".odt" is: application/vnd.oasis.opendocument.text
The file type extension for this MIME type is: .odt
*/
```
.

#### <a name="example2">2.</a> Build, Serialize, and Parse a `MimeType` instance:
```csharp
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
```
.

#### <a name="example3">3.</a> Format a `MimeType` instance into a standards-compliant string using several options:
```csharp
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
```

.
#### <a name="example4">4.</a> Compare `MimeType` instances for equality:


```csharp
using FolkerKinzel.MimeTypes;

namespace Examples;

public static class EqualityExample
{
    public static void Example()
    {
        const string media1 = "text/plain; charset=us-ascii";
        const string media2 = "TEXT/PLAIN";
        const string media3 = "TEXT/HTML";
        const string media4 = "text/plain; charset=iso-8859-1";
        const string media5 = "TEXT/PLAIN; CHARSET=ISO-8859-1";
        const string media6 = "text/plain; charset=iso-8859-1; other-parameter=other_value";
        const string media7 = "text/plain; OTHER-PARAMETER=other_value; charset=ISO-8859-1";
        const string media8 = "text/plain; charset=iso-8859-1; other-parameter=OTHER_VALUE";

        if (MimeType.Parse(media1) == MimeType.Parse(media2) &&
           MimeType.Parse(media2) != MimeType.Parse(media3) &&
           MimeType.Parse(media2) != MimeType.Parse(media4) &&
           MimeType.Parse(media2).Equals(MimeType.Parse(media4), ignoreParameters: true) &&
           MimeType.Parse(media4) == MimeType.Parse(media5) &&
           MimeType.Parse(media4) != MimeType.Parse(media6) &&
           MimeType.Parse(media6) == MimeType.Parse(media7) &&
           MimeType.Parse(media6) != MimeType.Parse(media8))
        {
            Console.WriteLine("Success");
        }
        else
        {
            Console.WriteLine("Error");
        }
    }
}
// Console Output: Success
```