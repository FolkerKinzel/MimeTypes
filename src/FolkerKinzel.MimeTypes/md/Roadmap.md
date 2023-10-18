# FolkerKinzel.MimeTypes
## Roadmap
### 5.0.2
- [x] Dependency update
- [x] Cleanup: Remove obsolete symbols
- [x] Performance: Let `MimeTypeInfo.TryParseInternal` fail faster when called with `null` or an empty string.

### 5.0.1
- [x] Performance: Calculate StringBuilder.Capacity rather than estimate it.
- [x] Performance: ToString(options) should not allocate a StringBuilder if `MimeFormats.IgnoreParameters` is set.
- [x] Fixes a bug, that `MimeTypeInfo.AppendTo` produces unexpected results when called on `MimeTypeInfo.Empty`
- [x] Fixes a bug, that `MimeTypeParameterInfo.AppendTo` produces unexpected results when called on `MimeTypeParameterInfo.Empty`

### 4.0.0
- [ ] Look whether MimeCache could work with less string allocation.

### 4.0.0-rc.1
- [x] see the [Release Notes](https://github.com/FolkerKinzel/MimeTypes/releases/tag/v4.0.0-rc.1)

### 3.0.0-beta.1
- [x] Replace `FormattingOptions` with `MimeFormats`
- [x] Rename the parameters `alwaysUrlEncoded` in `MimeTypeParameter.ToString(...)` and `MimeTypeParameter.AppendTo(...)` into `urlFormat`

### 2.0.0-beta.5
- [x] Fix a bug that choosing `FormattingOptions.AlwaysUrlEncoded` did mark parameters as URL-encoded even if they didn't contain any character to escape.

### 2.0.0-beta.3
- [x] Dependency update
- [x] Higher Code Coverage
- [x] Refactor ParameterSplitter
- [x] Faster validation of token names
- [x] Fix a bug that splitted HTTP quoted strings could not be read correctly when the last character of a splitted chunk is a masking backslash.
- [x] Fix known issue: If a splitted parameter contains url encoded and quoted parts, and if in a quoted part a url encoded escape sequence is, e.g., '%7e'
the decoder tries to decode it. (%7 or 7E causes no problems.)
- [x] Remove the German resource file.
- [x] Remove unused resources
- [x] `MimeType.AppendTo(...)` and `MimeTypeParameter.AppendTo(...)` should better return the StringBuilder instead of void.


### 2.0.0-beta.2
- [x] Mark ParameterModel as `sealed`
- [x] Mark MimeTypeBuilder as `sealed`
- [x] Fix a bug that Internet Media Type parameters including SPACE characters have not been enclosed with double quotes.


### 2.0.0-beta.1
- [x] .NET 7 support
- [x] Dependency update
- [x] Data update.
- [x] High code coverage.
- [x] **Breaking Change:** Make `MimeTypeEqualityComparer` an abstract class that provides two singletons: `MimeTypeEqualityComparer.Default` and `MimeTypeEqualityComparer.IgnoreParameters`.
- [x] Implement `MimeType.Parse(ReadOnlyMemory<char>)`.
- [x] **Breaking Change:** Refactor `MimeType.TryParse(ref ReadOnlyMemory<char>, out MimeType)` to `MimeType.TryParse(ReadOnlyMemory<char>, out MimeType)` in order to make a copy of `ReadOnlyMemory<char>`.
- [x] **Breaking Change:** Make `MimeType.Parameters` a method in order to show that iterating through the parameters can be an expensive operation under certain circumstances.
- [x] **Breaking Change:** Rename `MimeTypeParameter.Charset` to `MimeTypeParameter.CharSet`.
- [x] **Breaking Change:** Rename `MimeTypeParameter.IsCharsetParameter` to `MimeTypeParameter.IsCharSetParameter`.
- [x] **Breaking Change:** Rename `MimeTypeParameter.IsAsciiCharsetParameter` to `MimeTypeParameter.IsAsciiCharSetParameter`.
- [x] **Breaking Change:** Rename `MimeTypeParameterModel` to `ParameterModel`.
- [x] **Breaking Change:** Rename `MimeTypeParameterModelDictionary` to `ParameterModelDictionary`.
- [x] **Breaking Change:** Rename the parameter name `urlEncodedValue` in `MimeTypeParameter.AppendTo(...)` to `alwaysUrlEncoded`.
- [x] **Breaking Change:** `MimeType.AppendTo(...)` returns `void` now.
- [x] **Breaking Change:** `MimeTypeParameter.AppendTo(...)` returns `void` now.
- [x] **Breaking Change:** `MimeCache.EnlargeCapacity()` doesn't throw `ArgumentOutOfRangeException` anymore.
- [x] **Breaking Change:** Rename `MimeTypeFormattingOptions` to `FormattingOptions`.
- [x] **Breaking Change:** Refactor `FormattingOptions` to combine all otions with `FormattingOptions.IncludeParameters` except `FormattingOptions.None`.
- [x] **Breaking Change:** Make the ctor `MimeType(string, string, ParameterModelDictionary?)` internal.
- [x] **Breaking Change:** Make `ParameterModel` an internal class.
- [x] **Breaking Change:** Make `ParameterModelDictionary` internal
- [x] **Breaking Change:** 
- [x] Implement an overload `MimeTypeParameter.ToString(bool)`;
- [x] Make the second parameter of `MimeType.AppendTo(...)` optional.
- [x] Implement `MimeTypeBuilder`.


### 1.0.0-beta.4
- [x] Higher code coverage.

### 1.0.0-alpha.1
- [x] Release on nuget.

