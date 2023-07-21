# FolkerKinzel.MimeTypes
## Roadmap
### 2.0.0-beta.3
- [ ] Higher Code Coverage
- [ ] Refactor ParameterSplitter
- [ ] Faster validation of token names
- [x] Fix a bug that splitted HTTP quoted strings could not be read correctly when the last character of a splitted chunk is a masking backslash.
- [ ] Known issue: If a splitted parameter contains url encoded and quoted parts, and if in a quoted part a url encoded escape sequence is, e.g., '%7e'
the decoder tries to decode it. (%7 or 7E causes no problems.)

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

