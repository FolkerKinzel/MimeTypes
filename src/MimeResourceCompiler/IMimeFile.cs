namespace MimeResourceCompiler;

/// <summary>
/// Represents the resource file "Mime.csv", which is used to retrieve an appropriate file type extension for a given MIME type.
/// </summary>
public interface IMimeFile : ICompiledFile
{
    /// <summary>
    /// Returns the current file position in Mime.csv.
    /// </summary>
    /// <returns>The current file position in Mime.csv.</returns>
    long GetCurrentStreamPosition();
}
