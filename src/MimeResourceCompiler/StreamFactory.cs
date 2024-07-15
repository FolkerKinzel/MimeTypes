namespace MimeResourceCompiler;

/// <summary>
/// Encapsulates functionality to create FileStreams to write the output.
/// </summary>
public interface IStreamFactory
{
    /// <summary>
    /// Creates a FileStream to write a file named like <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">A file name without path information.</param>
    /// <returns>A FileStream to write a file named like <paramref name="fileName"/>.</returns>
    Stream CreateWriteStream(string fileName);
}

/// <summary>
/// Encapsulates functionality to create FileStreams to write the output.
/// </summary>
public sealed class StreamFactory(string outputDirectory, ILogger log) : IStreamFactory
{
    private readonly string _outputDirectory = outputDirectory;
    private readonly ILogger _log = log;

    /// <summary>
    /// Creates a FileStream to write a file named like <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">A file name without path information.</param>
    /// <returns>A FileStream to write a file named like <paramref name="fileName"/>.</returns>
    public Stream CreateWriteStream(string fileName)
    {
        var fs = new FileStream(Path.Combine(_outputDirectory, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
        _log.Debug("Created FileStream for {fileName}.", fileName);
        return fs;
    }
}
