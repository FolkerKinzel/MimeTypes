# FolkerKinzel.MimeTypes
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.MimeTypes)](https://www.nuget.org/packages/FolkerKinzel.MimeTypes/)
[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)

### .NET Library that supports working with Internet Media Types ("MIME Types")
[Project Reference and Release Notes](https://github.com/FolkerKinzel/MimeTypes/releases/tag/v3.0.0-beta.1)

The library contains:
- The static `MimeType` class allows to retrieve an appropriate file type extension for a given
- `MimeType` struct: Represents a MIME type ("Internet Media Type") according to [RFC 2045](https://datatracker.ietf.org/doc/html/rfc2045#section-5.1) and [RFC 2231](https://datatracker.ietf.org/doc/html/rfc2231.html). The struct can be created from a file type extension or parsed from an Internet Media Type string or ReadOnlyMemory&lt;Char&gt;.
- The MimeType struct allows 
  - to retrieve an appropriate file type extension for its content automatically,
  - to serialize its content as an Internet Media Type string according to the standards,
  - to compare its content with other Internet Media Types for equality.
- The `MimeTypeBuilder` class offers a fluent API to build MimeTypeInfo instances from scratch.
- The `FormattingOptions` enum allows a featured string serialization of MimeTypeInfo instances.

The library is designed to support performance and small heap allocation. To have a a detailed explanation where the data for the file type extension parser comes from and how it is compiled, have a look at [MimeResourceCompiler](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/MimeResourceCompiler/Program.cs) 
that is part of this repository.

[Version History](https://github.com/FolkerKinzel/MimeTypes/releases)


### Code Examples
1. [Get an Internet Media Type ("MIME type") string from a file type extension and vice versa](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/FileExtensionExample.cs)
2. [Build, serialize, and parse a MimeTypeInfo instance](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/BuildAndParseExample.cs)
3. [Format a MimeTypeInfo instance into a standards-compliant string using several options](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/FormattingOptionsExample.cs)
4. [Compare MimeTypeInfo instances](https://github.com/FolkerKinzel/MimeTypes/blob/master/src/Examples/EqualityExample.cs)
