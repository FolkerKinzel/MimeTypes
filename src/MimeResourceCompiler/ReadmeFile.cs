namespace MimeResourceCompiler;

/// <summary>
/// Represents the readme file Readme.txt.
/// </summary>
public class ReadmeFile(IOutputDirectory outputDirectory, IResourceLoader resourceLoader, ILogger log)
{
    private const string FILENAME = "Readme.txt";
    private readonly IOutputDirectory _outputDirectory = outputDirectory;
    private readonly IResourceLoader _resourceLoader = resourceLoader;
    private readonly ILogger _log = log;

    /// <summary>
    /// Creates the file Readme.txt.
    /// </summary>
    public void Create()
    {
        string path = Path.Combine(_outputDirectory.FullName, FILENAME);
        File.WriteAllBytes(path, LoadReadmeFile());
        _log.Debug("{0} successfully created.", path);
    }

    /// <summary>
    /// Loads Readme.txt from the resources.
    /// </summary>
    /// <returns>Readme.txt as byte array.</returns>
    private byte[] LoadReadmeFile()
    {
        using Stream? stream = _resourceLoader.GetResourceStream(FILENAME);

        byte[] arr = new byte[stream.Length];
        _ = stream.Read(arr, 0, arr.Length);

        _log.Debug("{0} successfully loaded from the resources.", FILENAME);
        return arr;
    }
}
