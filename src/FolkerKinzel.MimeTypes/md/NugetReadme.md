[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)

### .NET library that supports the work with Internet Media Types ("MIME Types").
The library contains:
* `readonly struct MimeType`: Represents a MIME type ("Internet Media Type") according to RFC 2045, RFC 2046 and RFC 2184. The struct can be created automatically from a file type extension or parsed from a `String` or a `ReadOnlyMemory<Char>`.
* The class `MimeTypeBuilder` offers a fluent API to build `MimeType` instances from scratch.
* The `MimeType` struct is able to retrieve an appropriate file type extension for its content automatically.

The library is designed to support performance and small heap allocation.

Read the [Project Reference and Release Notes](https://github.com/FolkerKinzel/MimeTypes/releases/tag/v2.0.0-beta.1) for details.

.
- [Version History](https://github.com/FolkerKinzel/MimeTypes/releases)



