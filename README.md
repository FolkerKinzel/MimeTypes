# FolkerKinzel.MimeTypes
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.MimeTypes)](https://www.nuget.org/packages/FolkerKinzel.MimeTypes/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)
[![Stand With Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://stand-with-ukraine.pp.ua)

## .NET Library that supports working with Internet Media Types ("MIME Types")

[Project Reference](https://folkerkinzel.github.io/MimeTypes/reference/)

- The static `MimeString` class works on strings and allows to convert file names into Internet Media Types ("MIME types") or Internet Media Types into appropriate file type extensions.
- The `MimeType` class allows:
  -  Building instances from scratch using a fluent API,
  -  Parsing and validating Internet Media Types,
  -  Editing Internet Media Type parameters,
  -  Serializing Internet Media Types as strings according to the standards (see [RFC 2045](https://datatracker.ietf.org/doc/html/rfc2045#section-5.1) and [RFC 2231](https://datatracker.ietf.org/doc/html/rfc2231.html)) using several formatting options,
  -  Retrieving a file type extension.
- The `MimeTypeInfo` struct allows memory efficient parsing, validating, and reformatting of Internet Media Type strings and allows to retrieve an appropriate file type extension.

The library is designed to support performance and small heap allocation. 

The data for the file type extension parser is collected from:
1. [Apache](https://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types)
2. [jshttp/mime-db](https://github.com/jshttp/mime-db)
2. own research.

(The collected data was considered in the specified order. Have a detailed explanation about how this data is ordered and compiled in
the comments of [MimeResourceCompiler](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/MimeResourceCompiler/Program.cs), which is
part of this repository.)

[Version History](https://github.com/FolkerKinzel/MimeTypes/releases)


### Code Examples
1. [Getting an Internet Media Type ("MIME type") string from a file type extension and vice versa](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/FileExtensionExample.cs)
2. [Building, serializing, parsing, and editing of  MimeType instances](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/BuildAndParseExample.cs)
3. [Formatting a MimeType instance into a standards-compliant string using several options](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/FormattingOptionsExample.cs)
4. [Comparison of MimeType instances](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/EqualityExample.cs)
5. [Efficient parsing of an Internet Media Type String](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/MimeTypeInfoExample.cs)
