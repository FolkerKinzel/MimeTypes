[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)


### .NET Library that supports working with Internet Media Types ("MIME Types")
[Project Reference and Release Notes](https://github.com/FolkerKinzel/MimeTypes/releases/tag/v4.0.0-rc.1)

- The static `MimeString` class works on strings and allows to convert file names into Internet Media Types ("MIME types") or Internet Media Types into appropriate file type extensions.
- The `MimeType` class allows:
  -  Building instances from scratch using a fluent API,
  -  Parsing and validating Internet Media Types,
  -  Editing Internet Media Type parameters,
  -  Serializing Internet Media Types as strings according to the standards (see [RFC 2045](https://datatracker.ietf.org/doc/html/rfc2045#section-5.1) and [RFC 2231](https://datatracker.ietf.org/doc/html/rfc2231.html)) using several formatting options,
  -  Retrieving a file type extension.
- The `MimeTypeInfo` struct allows memory efficient parsing and validating of Internet Media Type strings.

The library is designed to support performance and small heap allocation.

[See code examples at GitHub](https://github.com/FolkerKinzel/MimeTypes)

[Version History](https://github.com/FolkerKinzel/MimeTypes/releases)



