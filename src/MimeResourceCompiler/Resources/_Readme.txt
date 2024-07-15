Usage of the files:
==================

Copy the files
- Mime.csv
- MimeIdx.csv
- Extension.csv

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
	TopLevelMediaType ByteIndex NumberOfLines
	ByteIndex: The Byte position in Mime.csv where the Top Level Media Type starts.
	NumberOfLines: The maximum number of lines to read for this Top Level Media Type.
- Extension.csv:
	Provides the data used to retrieve an appropriate MIME type for a given file type extension. 
	(The file type extension is the key.)