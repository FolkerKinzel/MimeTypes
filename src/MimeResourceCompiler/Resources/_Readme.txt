Usage of the files:
==================

Copy the files
- Extension.csv
- ExtensionIdx.csv
- Mime.csv
- MimeIdx.csv

and paste it to the directory FolkerKinzel.Uris/Resources by replacing the existing files.

Start Visual Studio and make sure that the Build Action of all pasted files is set to "Embedded Resource".

File Description:
=================
- Mime.csv: 
	Provides the data used to retrieve an appropriate file type extension for a given MIME type. 
	(The MIME type is the key.)
	The MIME types are grouped by their Top Level Media Type for faster access.
- MimeIdx.csv:
	Provides an index for Mime.csv for faster reading. The format is
	<top-level media type> <byte index> <number of lines>
	Byte index: The byte position in Mime.csv where the specified top-level media type starts.
	Number of lines: The maximum number of lines to read for this top-level media type.
- Extension.csv:
	Provides the data used to retrieve an appropriate MIME type for a given file type extension. 
	(The file type extension is the key.)
- ExtensionIdx.csv:
	Provides an index for Extension.csv for faster reading. The format is
	<first letter of the extension> <byte index> <number of lines>
	Byte index: The byte position in Extension.csv where the extensions with the specified first letter start.
	Number of lines: The maximum number of lines to read for extensions with the specified first letter.