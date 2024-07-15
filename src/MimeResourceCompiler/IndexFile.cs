namespace MimeResourceCompiler;

/// <summary>
/// Represents the index file MimeIdx.csv.
/// </summary>
public interface IIndexFile : IDisposable
{
    /// <summary>
    /// Writes the index for a specified key to the file.
    /// </summary>
    /// <param name="key">The key for that the index should be used.</param>
    /// <param name="startPosition">Byte index in the compiled file where the <paramref name="key"/> range starts.</param>
    /// <param name="rowsCount">The number of rows of <paramref name="key"/> range in the compiled file.</param>
    void WriteNewIndexEntry(string key, long startPosition, int rowsCount);

    string FileName { get; }
}

/// <summary>
/// Represents the index file MimeIdx.csv.
/// </summary>
public sealed class IndexFile : IDisposable, IIndexFile
{
    private const string NEW_LINE = "\n";
    private const char SEPARATOR = ' ';

    private readonly StreamWriter _writer;
    private readonly ILogger _log;


    public IndexFile(string fileName, IStreamFactory streamFactory, ILogger log)
    {
        FileName = fileName;
        Stream stream = streamFactory.CreateWriteStream(FileName);

        _writer = new StreamWriter(stream)
        {
            NewLine = NEW_LINE
        };

        _log = log;
    }

    public string FileName { get; }


    /// <summary>
    /// Writes the index for a specified key to the file.
    /// </summary>
    /// <param name="key">The key for that the index should be used.</param>
    /// <param name="startPosition">Byte index in the compiled file where the <paramref name="key"/> range starts.</param>
    /// <param name="rowsCount">The number of rows of <paramref name="key"/> range in the compiled file.</param>
    public void WriteNewIndexEntry(string key, long startPosition, int rowsCount)
    {
        _writer.Write(key);
        _writer.Write(SEPARATOR);
        _writer.Write(startPosition);
        _writer.Write(SEPARATOR);
        _writer.WriteLine(rowsCount);
    }

    ///// <summary>
    ///// Writes the rows count of the current media type in Mime.csv to the index file.
    ///// </summary>
    ///// <param name="rowsCount">The number of rows of a media type in Mime.csv.</param>
    //public void WriteRowsCount(int linesCount) => _writer.WriteLine(linesCount);

    public void Dispose()
    {
        _writer?.Close();
        GC.SuppressFinalize(this);
        _log.Debug("{0} closed.", FileName);
    }

}
