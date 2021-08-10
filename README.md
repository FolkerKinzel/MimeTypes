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
content of MIME types without having to allocate a lot of temporary Strings.

Read the [Project Reference](https://github.com/FolkerKinzel/MimeTypes/blob/master/ProjectReference/1.0.0-alpha.1/FolkerKinzel.MimeTypes.Reference.en.chm) for details.

> IMPORTANT: On some systems the content of the .CHM file is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox (if it is present) in the lower right corner of the General tab in the Properties dialog.

.
- [Version History](https://github.com/FolkerKinzel/MimeTypes/releases)
