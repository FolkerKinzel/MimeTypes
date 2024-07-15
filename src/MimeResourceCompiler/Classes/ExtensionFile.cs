namespace MimeResourceCompiler.Classes;

/// <summary>
/// Represents the compiled file "Extension.csv", which is used to retrieve the MIME type for a given file type extension.
/// </summary>
public sealed class ExtensionFile(IStreamFactory streamFactory, ILogger log) : CompiledFile(streamFactory, log)
{

    /// <summary>
    /// The filename of the compiled file. (Without path information.)
    /// </summary>
    public override string FileName => "Extension.csv";

}
