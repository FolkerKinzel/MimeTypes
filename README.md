# FolkerKinzel.MimeTypes
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.MimeTypes)](https://www.nuget.org/packages/FolkerKinzel.MimeTypes/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)

## .NET Library that supports working with Internet Media Types ("MIME Types")

[Project Reference and Release Notes](https://github.com/FolkerKinzel/MimeTypes/releases/tag/v5.2.0-beta.1)

- The static `MimeString` class works on strings and allows to convert file names into Internet Media Types ("MIME types") or Internet Media Types into appropriate file type extensions.
- The `MimeType` class allows:
  -  Building instances from scratch using a fluent API,
  -  Parsing and validating Internet Media Types,
  -  Editing Internet Media Type parameters,
  -  Serializing Internet Media Types as strings according to the standards (see [RFC 2045](https://datatracker.ietf.org/doc/html/rfc2045#section-5.1) and [RFC 2231](https://datatracker.ietf.org/doc/html/rfc2231.html)) using several formatting options,
  -  Retrieving a file type extension.
- The `MimeTypeInfo` struct allows memory efficient memory efficient parsing, validating, and reformatting of Internet Media Type strings and allows to retrieve an appropriate file type extension.

The library is designed to support performance and small heap allocation. To have a a detailed explanation where the data for the file type extension parser comes from and how it is compiled, have a look at [MimeResourceCompiler](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/MimeResourceCompiler/Program.cs) 
that is part of this repository.

[Version History](https://github.com/FolkerKinzel/MimeTypes/releases)


### Code Examples
1. [Getting an Internet Media Type ("MIME type") string from a file type extension and vice versa](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/FileExtensionExample.cs)
2. [Building, serializing, parsing, and editing of  MimeType instances](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/BuildAndParseExample.cs)
3. [Formatting a MimeType instance into a standards-compliant string using several options](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/FormattingOptionsExample.cs)
4. [Comparison of MimeType instances](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/EqualityExample.cs)
5. [Efficient parsing of an Internet Media Type String](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/MimeTypeInfoExample.cs)
