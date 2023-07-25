First some **notes in respect to the users of the beta versions**:

You may be wondering why a major version is being released as a preview again. I'm using the library myself in other projects and have noticed that finding appropriate file type extensions for given Internet Media Type strings is more complicated than necessary. 

Extensive changes to the public interface were required to fix this issue. I think the best way to deal with this was to release the changes as soon as possible to upset as few users as possible. I apologize.

The most important changes are:
- The `MimeTypeStruct` has been renamed to `MimeTypeInfo` in order to show its use more clearly.
- `MimeType` is a static class now that works on strings and has the task to convert file type extensions into Internet Media Types and vice versa.
- `MimeTypeBuilder` got additional methods:
```csharp
MimeTypeBuilder Create(in MimeTypeInfo info);
MimeTypeBuilder RemoveKey(string key);
```

>**Project reference:** On some systems, the content of the CHM file in the Assets is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox - if it is present - in the lower right corner of the General tab in the Properties dialog.