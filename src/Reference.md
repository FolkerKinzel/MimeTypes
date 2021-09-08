[![GitHub](https://img.shields.io/github/license/FolkerKinzel/MimeTypes)](https://github.com/FolkerKinzel/MimeTypes/blob/master/LICENSE)

The library contains:
* `readonly struct MimeType`: Represents a MIME type ("Internet Media Type") according to RFC 2045, RFC 2046 and RFC 2184. The struct can be created automatically from a file type extension, parsed from a String or a `ReadOnlyMemory<Char>` or build from its parts. It is able to retrieve an appropriate file type extension for its content.

The library makes extensive use of ReadOnlySpan&lt;Char&gt; and ReadOnlyMemory&lt;Char&gt; to examine the content of MIME types without having to allocate a lot of temporary Strings. A strong validation is built in for security reasons.

Read the [Project Reference](https://github.com/FolkerKinzel/MimeTypes/blob/master/ProjectReference/1.0.0-beta.2/FolkerKinzel.MimeTypes.Reference.en.chm) for details.

> IMPORTANT: On some systems the content of the .CHM file is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox (if it is present) in the lower right corner of the General tab in the Properties dialog.

.
- [Version History](https://github.com/FolkerKinzel/MimeTypes/releases)



