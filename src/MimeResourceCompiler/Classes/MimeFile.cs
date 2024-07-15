namespace MimeResourceCompiler.Classes;

/// <summary>
/// Represents the resource file "Mime.csv", which is used to retrieve an appropriate file type extension for a given MIME type.
/// </summary>
public sealed class MimeFile(IStreamFactory streamFactory, ILogger log)
    : CompiledFile(streamFactory, log), IMimeFile
{

    /// <summary>
    /// The filename of the compiled file. (Without path information.)
    /// </summary>
    public override string FileName => "Mime.csv";

    /// <summary>
    /// Returns the current file position in Mime.csv.
    /// </summary>
    /// <returns>The current file position in Mime.csv.</returns>
    public long GetCurrentStreamPosition()
    {
        _writer.Flush();
        return _writer.BaseStream.Position;
    }


}
