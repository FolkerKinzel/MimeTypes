namespace MimeResourceCompiler.Classes;

/// <summary>
/// Encapsulates functionality to create FileStreams to write the output.
/// </summary>
public sealed class StreamFactory(IOutputDirectory outputDirectory, ILogger log) : IStreamFactory
{
    private readonly IOutputDirectory _outputDirectory = outputDirectory;
    private readonly ILogger _log = log;

    /// <summary>
    /// Creates a FileStream to write a file named like <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">A file name without path information.</param>
    /// <returns>A FileStream to write a file named like <paramref name="fileName"/>.</returns>
    public Stream CreateWriteStream(string fileName)
    {
        var fs = new FileStream(Path.Combine(_outputDirectory.FullName, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
        _log.Debug("Created FileStream for {fileName}.", fileName);
        return fs;
    }
}
