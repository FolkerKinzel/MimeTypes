# FolkerKinzel.MimeTypes
## Roadmap

### 2.0.0
- [x] .NET 7 support
- [x] Dependency update
- [x] Data update.
- [x] Higher code coverage.
- [x] **Breaking Change:** Make `MimeTypeEqualityComparer` an abstract class that provides two singletons: `MimeTypeEqualityComparer.Default` and `MimeTypeEqualityComparer.IgnoreParameters`.
- [x] Implement `MimeType.Parse(ReadOnlyMemory<char>)`.
- [x] **Breaking Change:** Refactor `MimeType.TryParse(ref ReadOnlyMemory<char>, out MimeType)` to `MimeType.TryParse(ReadOnlyMemory<char>, out MimeType)` in order to make a copy of `ReadOnlyMemory<char>`.
- [x] **Breaking Change:** Rename `MimeType.Parameters` to `MimeType.GetParameters()` and make it a method in order to show that iteratin through the parameters can be an expensive operation under certain circumstances.

### 1.0.0-beta.4
- [x] Higher code coverage.

### 1.0.0-alpha.1
- [x] Release on nuget.

