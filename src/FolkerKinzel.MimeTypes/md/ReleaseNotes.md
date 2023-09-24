- **Breaking Change:** The content of the public constant `MimeCache.DefaultFileTypeExtension` 
has been changed to `.bin` in order to be aligned to the behavior of `System.IO.Path.GetExtension`. (In former versions it was `bin`.) **Don't forget to check existing code whether the constant had been used in it.**
- **Breaking Change:** `MimeTypeInfo.ToString()` and `MimeTypeParameterInfo.ToString()` now return
a standard formatted string representation of their content rather than their struct names.
- Dependency update
- New methods that allow to efficiently reformat a parsed Internet Media Type:
```csharp
string MimeTypeInfo.ToString(MimeFormats, int);
StringBuilder MimeTypeInfo.AppendTo(StringBuilder, MimeFormats, int);
string MimeTypeParameterInfo.ToString(bool);
StringBuilder MimeTypeParameterInfo.AppendTo(StringBuilder, bool);
```
.

>**Project reference:** On some systems, the content of the CHM file in the Assets is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox - if it is present - in the lower right corner of the General tab in the Properties dialog.