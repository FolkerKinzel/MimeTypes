# FolkerKinzel.MimeTypes
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.MimeTypes)](https://www.nuget.org/packages/FolkerKinzel.MimeTypes/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)

.NET library that supports the work with Internet Media Types ("MIME Types").

The library contains:
* `readonly struct MimeType`: Represents a MIME type ("Internet Media Type") according 
to RFC 2045, RFC 2046 and RFC 2184. The struct can be created automatically from a file type 
extension or parsed from a `String` or a `ReadOnlyMemory<Char>`. It is 
able to find an appropriate file type extension for its content. (Have a look at
 [MimeResourceCompiler](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/MimeResourceCompiler/Program.cs) that is part of the repository to
have a detailed explanation where the data comes from and how it is compiled.)

The library makes extensive use of ReadOnlySpan&lt;Char&gt; and ReadOnlyMemory&lt;Char&gt; to build and examine the 
content of MIME types without having to allocate a lot of temporary Strings. A strong validation is built in for 
security reasons.

Read the [Project Reference](https://github.com/FolkerKinzel/MimeTypes/blob/master/ProjectReference/1.0.0-beta.1/FolkerKinzel.MimeTypes.Reference.en.chm) for details.

> IMPORTANT: On some systems the content of the .CHM file is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox (if it is present) in the lower right corner of the General tab in the Properties dialog.

### Examples
Getting `MimeType` instances by parsing file type extensions and getting appropriate file type extensions
from `MimeType` instances:
```csharp
using System;
using System.IO;
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
 */
```
.

Building and parsing `MimeType` instances:
```csharp
using System;
using FolkerKinzel.MimeTypes;

namespace Examples
{
    public static class BuildAndParseExample
    {
        public static void Example()
        {
            var dic = new MimeTypeParameterModelDictionary()
            {
                new MimeTypeParameterModel("first-parameter",
                "This is a very long parameter, which will be wrapped according to RFC 2184." +
                Environment.NewLine +
                "It contains also a few Non-ASCII-Characters: \u00E4\u00D6\u00DF.", "en"),
                new MimeTypeParameterModel("second-parameter", "Parameter with  \\, = and \".")
            };

            var mimeType1 = new MimeType("application", "x-stuff", dic);
            string s = mimeType1.ToString(
                        MimeTypeFormattingOptions.LineWrapping | MimeTypeFormattingOptions.Default);
            Console.WriteLine(s);

            var mimeType2 = MimeType.Parse(s);

            Console.WriteLine();
            Console.WriteLine($"Media Type: {mimeType2.MediaType.ToString()}");
            Console.WriteLine($"Sub Type:   {mimeType2.SubType.ToString()}");

            int parameterCounter = 1;
            foreach (MimeTypeParameter parameter in mimeType2.Parameters)
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
It contains also a few Non-ASCII-Characters: ‰÷ﬂ.
Language: en

Parameter 2:
============
Key:      second-parameter
Value:    Parameter with  \, = and ".
Language:
 */
```
.
    
Comparing `MimeType` instances for equality:
```csharp
using System;
using FolkerKinzel.MimeTypes;

namespace Examples
{
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
}
// Console Output: Success

```
.

- [Version History](https://github.com/FolkerKinzel/MimeTypes/releases)
