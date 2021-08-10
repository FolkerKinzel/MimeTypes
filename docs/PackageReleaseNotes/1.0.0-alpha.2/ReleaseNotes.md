# FolkerKinzel.MimeTypes 1.0.0-alpha.2
## Package Release Notes
- Implements `IComparable<MimeTypeParameter>` in `MimeTypeParameter`.
- Better algorythm for MimeType.Equals and MimeType.GetHashCode.
- New class MimeTypeEqualityComparer.
- Ability to filter out comments which are embedded into MIME types when parsing. (RFC 822).