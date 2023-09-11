- **Important update:**
    Fixes a 2 bugs in `MimeTypeInfo`:
    1. A plus sign in a not URL-encoded continuation of a URL-encoded splitted parameter could have accidentally been changed to a SPACE sign by the decoder.
    2. A URL-encoded parameter value was not encoded if '+' signs had been the only escape characters.

- Dependency update
- Performance optimizations


>**Project reference:** On some systems, the content of the CHM file in the Assets is blocked. Before opening the file right click on the file icon, select Properties, and check the "Allow" checkbox - if it is present - in the lower right corner of the General tab in the Properties dialog.