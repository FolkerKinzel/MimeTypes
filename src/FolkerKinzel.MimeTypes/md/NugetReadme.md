[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)


### .NET Library that supports working with Internet Media Types ("MIME Types")
[Project Reference and Release Notes](https://github.com/FolkerKinzel/MimeTypes/releases/tag/v2.0.0-beta.3)

The library contains:
- `MimeType` struct: Represents a MIME type ("Internet Media Type") according to RFC 2045, RFC 2046 and RFC 2231. The struct can be created from a file type extension or parsed from an Internet Media Type string or ReadOnlyMemory&lt;Char&gt;.
- The MimeType struct allows 
  - to retrieve an appropriate file type extension for its content automatically,
  - to serialize its content as an Internet Media Type string according to the standards,
  - to compare its content with other Internet Media Types for equality.
- The class `MimeTypeBuilder` offers a fluent API to build MimeType instances from scratch.
- The `FormattingOptions` enum allows a featured string serialization of MimeType instances.

The library is designed to support performance and small heap allocation.

[See code examples at GitHub](https://github.com/FolkerKinzel/MimeTypes)

[Version History](https://github.com/FolkerKinzel/MimeTypes/releases)



