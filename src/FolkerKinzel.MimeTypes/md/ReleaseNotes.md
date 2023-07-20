Version 2 is a complete revision of the library.

It offers
- .NET 7 support
- Dependency update
- Data update.
- High code coverage.
- A new class `MimeTypeBuilder` which has a fluent API that allows to build `MimeType` instances from scratch.
- A new overload of `MimeTypeParameter.ToString(bool alwaysUrlEncoded)` which allows to force a URL encoded string representation.
- A new overload `MimeType.Parse(ReadOnlyMemory<char>)`.
- A lot of **Breaking Changes** that increase the usability of the library:
  - `MimeTypeEqualityComparer` is now an abstract class that provides two singletons: `MimeTypeEqualityComparer.Default` and `MimeTypeEqualityComparer.IgnoreParameters`.
  - `MimeType.TryParse(ref ReadOnlyMemory<char>, out MimeType)` to `MimeType.TryParse(ReadOnlyMemory<char>, out MimeType)` in order to make a copy of the passed `ReadOnlyMemory<char>`.
  - The property `MimeType.Parameters` has been refactored into a method in order to show that iterating through the parameters can be an expensive operation under certain circumstances.
  - The classes `MimeTypeParameterModel` and `MimeTypeParameterModelDictionary` as well as the constructor `MimeType(string, string, MimeTypeParameterModelDictionary?)` have been made internal because since we have `MimeTypeBuilder` they are no longer needed.
  - The second Parameter of `MimeType.AppendTo(StringBuilder, FormattingOptions, int)` is optional now and defaults to `FormattingOptions.Default`. The return value of this method has been changed to `void`.
  - The method `MimeTypeParameter.AppendTo(...)` has been refactored to return `void`.
  - `MimeCache.EnlargeCapacity()` doesn't throw any `ArgumentOutOfRangeException` anymore.
  - The enum `MimeTypeFormattingOptions` has been replaced with `FormattingOptions` which combines all otions with `FormattingOptions.IncludeParameters`, except `FormattingOptions.None`, in order to protect users from unexpected results.
  - In order to have a more consistent naming, some identifiers have been renamed:
    - `MimeTypeParameter.Charset` into `MimeTypeParameter.CharSet`.
    - `MimeTypeParameter.IsCharsetParameter` into `MimeTypeParameter.IsCharSetParameter`.
    - `MimeTypeParameter.IsAsciiCharsetParameter` into `MimeTypeParameter.IsAsciiCharSetParameter`.
    - The parameter `urlEncodedValue` in the method `MimeTypeParameter.AppendTo(...)` into `alwaysUrlEncoded`.

>#### Notes to user who build .NET Core 2.x/3.0 application based on the .NET Standard 2.0 part of the package:
>You might get a compiler error. This is caused by a Microsoft dependency. You can get rid of this error, if you copy `<SuppressTfmSupportBuildWarnings>true</SuppressTfmSupportBuildWarnings>` to a `<PropertyGroup>` of your project file (at own risk).

>**Project reference:** On some systems, the content of the CHM file in the Assets is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox - if it is present - in the lower right corner of the General tab in the Properties dialog.