- `MimeTypeParameterInfo` got two new properties:
```csharp
bool IsAsciiCharSetParameter { get; }
bool IsValueCaseSensitive { get; }
```

- `MimeTypeParameter` got four new properties:
```csharp
bool IsCharSetParameter { get; }
bool IsAccessTypeParameter { get; }
bool IsAsciiCharSetParameter { get; }
bool IsValueCaseSensitive { get; }
```


>**Project reference:** On some systems, the content of the CHM file in the Assets is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox - if it is present - in the lower right corner of the General tab in the Properties dialog.