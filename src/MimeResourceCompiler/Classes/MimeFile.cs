namespace MimeResourceCompiler.Classes;

/// <summary>
/// Represents the compiled file "Mime.csv", which is used to retrieve an appropriate file type extension for a given MIME type.
/// </summary>
public sealed class MimeFile(IStreamFactory streamFactory, ILogger log)
    : CompiledFile(streamFactory, log), IMimeFile
{

    /// <summary>
    /// The filename of the compiled file. (Without path information.)
    /// </summary>
    public override string FileName => "Mime.csv";

    


}
