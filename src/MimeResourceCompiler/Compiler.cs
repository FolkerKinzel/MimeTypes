namespace MimeResourceCompiler;

/// <summary>
/// Compiles the output.
/// </summary>
public sealed class Compiler : IDisposable
{
    //private const string DEFAULT_MIME_TYPE = "application/octet-stream";
    private const int LIST_CAPACITY = 2048;
    private readonly IApacheData _apacheData;
    private readonly IMimeDBData _mimeDBData;

    /// <summary>
    /// Represents the compiled file "Mime.csv", which is used to retrieve an appropriate file type extension for a given MIME type.
    /// </summary>
    private readonly ICompiledFile _mimeFile;
    private readonly IIndexFile _mimeIndexFile;

    /// <summary>
    /// Represents the compiled file "Extension.csv", which is used to retrieve the MIME type for a given file type extension.
    /// </summary>
    private readonly ICompiledFile _extensionFile;
    private readonly IIndexFile _extensionIndexFile;

    private readonly IResourceParser _defaultEntry;
    private readonly IResourceParser _addendum;
    private readonly ILogger _log;
    private bool _disposedValue;

    public Compiler(IApacheData apacheData,
                    IMimeDBData mimeDBData,
                    ICompiledFile mimeFile,
                    IIndexFile mimeIndexFile,
                    ICompiledFile extensionFile,
                    IIndexFile extensionIndexFile,
                    IResourceParser defaultEntry,
                    IResourceParser addendum,
                    ILogger log)
    {
        _apacheData = apacheData;
        _mimeDBData = mimeDBData;
        _mimeFile = mimeFile;
        _mimeIndexFile = mimeIndexFile;
        _extensionFile = extensionFile;
        _extensionIndexFile = extensionIndexFile;
        _defaultEntry = defaultEntry;
        _addendum = addendum;
        _log = log;

        _log.Debug("Compiler initialized.");
    }

    public void CompileResources()
    {
        _log.Debug("Start Compiling.");
        List<Entry> list = CollectData();

        _log.Debug("Start writing the data files.");

        CompileMimeFile(list);
        CompileExtensionFile(list);

        _log.Debug("Data files completely written.");
    }

    /// <summary>
    /// Compiles the file "Mime.csv", which is used to retrieve an appropriate file type extension for a given MIME type.
    /// </summary>
    private void CompileMimeFile(List<Entry> list)
    {
        _log.Debug("Start writing {0} and {1}.", _mimeFile.FileName, _mimeIndexFile.FileName);

        foreach (IGrouping<string, Entry> group in list.GroupBy(x => x.MimeType)
                                                       .Select(g => g.First())
                                                       .GroupBy(x => x.TopLevelMediaType, StringComparer.Ordinal))
        {
            _mimeIndexFile.WriteNewIndexEntry(group.Key, _mimeFile.GetCurrentStreamPosition(), group.Count());
            _mimeFile.WriteEntries(group);
        }

        _log.Debug("{0} and {1} successfully written.", _mimeFile.FileName, _mimeIndexFile.FileName);
    }

    private void CompileExtensionFile(List<Entry> list)
    {
        _log.Debug("Start writing {0} and {1}.", _extensionFile.FileName, _extensionIndexFile.FileName);

        foreach (IGrouping<char, Entry> group in list.GroupBy(x => x.Extension)
                                                     .Select(g => g.First())
                                                     .GroupBy(x => x.Extension[0]))
        {
            _extensionIndexFile.WriteNewIndexEntry(group.Key.ToString(), _extensionFile.GetCurrentStreamPosition(), group.Count());
            _extensionFile.WriteEntries(group);
        }

        _log.Debug("{0} and {1} successfully written.", _extensionFile.FileName, _extensionIndexFile.FileName);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #region private

    private List<Entry> CollectData()
    {
        _log.Debug("Start collecting the data.");
        var list = new List<Entry>(LIST_CAPACITY);
        CollectResourceFile(list, _defaultEntry);
        CollectApacheData(list);
        CollectMimeDBData(list);
        CollectResourceFile(list, _addendum);

        _log.Debug("Data completely collected.");

        return list;
    }

    private void CollectResourceFile(List<Entry> list, IResourceParser parser)
    {
        _log.Debug("Start parsing the resource {0}.", parser.FileName);

        int initialCount = list.Count;
        Entry? entry;
        while ((entry = parser.GetNextLine()) is not null)
        {
            list.Add(entry);
        }

        _log.Information("{0} entries parsed from {1} file.", list.Count - initialCount, parser.FileName);
        _log.Debug("The resource {0} has been completely parsed.", parser.FileName);
    }


    private void CollectApacheData(List<Entry> list)
    {
        _log.Debug("Start parsing the Apache data.");
        var tmp = new List<Entry>(1024);

        IEnumerable<Entry>? line;

        while ((line = _apacheData.GetNextLine()) != null)
        {
            tmp.AddRange(line);
        }

        _apacheData.Dispose();

        if (tmp.Count == 0)
        {
            throw new FormatException("The Apache file probably has a new format.");
        }

        _log.Information("{0} entries parsed from the Apache file.", tmp.Count);

        _log.Debug("Apache data completely parsed.");

        ValidateDefaultCsvEntries(list, tmp);
        list.AddRange(tmp);
    }

    private void ValidateDefaultCsvEntries(List<Entry> list, List<Entry> tmp)
    {
        _log.Debug("Start validating that all Default.csv entries are in the Apache file.");

        foreach (Entry entry in list)
        {
            if (!tmp.Contains(entry))
            {
                throw new InvalidOperationException($"The Entry {entry} in Default.csv is not part of the Apache file.");
            }
        }

        _log.Debug("Default.csv validation completed.");
    }

    private void CollectMimeDBData(List<Entry> list)
    {
        _log.Debug("Start parsing the mime-db data.");

        _mimeDBData.GetData(list);

        _log.Debug("mime-db data completely parsed.");
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _apacheData.Dispose();
                _defaultEntry.Dispose();
                _addendum.Dispose();
                _mimeIndexFile.Dispose();
                _mimeFile.Dispose();
                _extensionFile.Dispose();
                _extensionIndexFile.Dispose();
            }

            _disposedValue = true;

            _log.Debug("Compiler disposed.");
        }
    }

    #endregion
}
