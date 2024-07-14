namespace MimeResourceCompiler;

/// <summary>
/// Compiles the output.
/// </summary>
public sealed class Compiler : IDisposable
{
    private const string DEFAULT_MIME_TYPE = "application/octet-stream";
    private const int LIST_CAPACITY = 2048;
    private readonly IApacheData _apacheData;
    private readonly IMimeDBData _mimeDBData;
    private readonly IMimeFile _mimeFile;
    private readonly IIndexFile _indexFile;
    private readonly ICompiledFile _extensionFile;
    private readonly IResourceParser _defaultEntry;
    private readonly IResourceParser _addendum;
    private readonly ILogger _log;
    private readonly ICompressor _compressor;
    private bool _disposedValue;

    public Compiler(IApacheData apacheData,
                    IMimeDBData mimeDBData,
                    IMimeFile mimeFile,
                    IIndexFile indexFile,
                    ICompiledFile extensionFile,
                    IResourceParser defaultEntry,
                    IResourceParser addendum,
                    ILogger log,
                    ICompressor compressor)
    {
        _apacheData = apacheData;
        _mimeDBData = mimeDBData;
        _mimeFile = mimeFile;
        _indexFile = indexFile;
        this._extensionFile = extensionFile;
        this._defaultEntry = defaultEntry;
        _addendum = addendum;
        _log = log;
        _compressor = compressor;

        _log.Debug("Compiler initialized.");
    }

    public void CompileResources()
    {
        _log.Debug("Start Compiling.");
        List<Entry> list = CollectData();
        list = list
            .GroupBy(x => x.TopLevelMediaType, StringComparer.Ordinal)
            .SelectMany(group => group)
            .Distinct()
            .SkipWhile(x => x.MimeType.Equals(DEFAULT_MIME_TYPE, StringComparison.Ordinal))
            .ToList();

        _log.Debug("Start removing unreachable entries.");
        _compressor.RemoveUnreachableEntries(list);
        _log.Debug("Unreachable entries completely removed.");

        _log.Debug("Start writing the data files.");
        CompileMimeFile(list);
        CompileExtensionFile(list);

        _log.Debug("Data files completely written.");
    }

    private void CompileMimeFile(List<Entry> list)
    {
        _log.Debug("Start writing {0} and {1}.", _mimeFile.FileName, _indexFile.FileName);
        var comparer = new MimeTypeEqualityComparer();
        foreach (IGrouping<string, Entry> group in list.GroupBy(x => x.TopLevelMediaType, StringComparer.Ordinal))
        {
            _indexFile.WriteNewMediaType(group.Key, _mimeFile.GetCurrentStreamPosition(), group.Count());
            _mimeFile.WriteEntries(group.Distinct(comparer));
        }
        _log.Debug("{0} and {1} successfully written.", _mimeFile.FileName, _indexFile.FileName);
    }

    private void CompileExtensionFile(List<Entry> list)
    {
        _log.Debug("Start writing {0}.", _extensionFile.FileName);
        _extensionFile.WriteEntries(list.Distinct(new ExtensionEqualityComparer()));
        _log.Debug("{0}  successfully written..", _extensionFile.FileName);
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
        int initialCount = list.Count;
        IEnumerable<Entry>? line;
        while ((line = _apacheData.GetNextLine()) != null)
        {
            list.AddRange(line);
        }

        _apacheData.Dispose();

        int count = list.Count - initialCount;

        if (count == 0)
        {
            throw new FormatException("The Apache file probably has a new format.");
        }

        _log.Information("{0} entries parsed from the Apache file.", count);

        _log.Debug("Apache data completely parsed.");


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
                _indexFile.Dispose();
                _mimeFile.Dispose();
                _extensionFile.Dispose();
            }

            _disposedValue = true;

            _log.Debug("Compiler disposed.");
        }
    }

    #endregion
}
