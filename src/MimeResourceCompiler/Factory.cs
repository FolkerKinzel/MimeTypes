namespace MimeResourceCompiler;

/// <summary>
/// Builds the objects used in the program and takes care to release their resources.
/// </summary>
internal sealed class Factory : IDisposable
{
    private static readonly HttpClient _httpClient = new();

    private readonly Compiler _compiler;
    private readonly ReadmeFile _readmeFile;

    public Factory(string outDir, Options options, ILogger _logger)
    {
        var apacheData = new ApacheData(_httpClient, _logger.ForContext<ApacheData>());
        var mimeDBData = new MimeDBData(_httpClient, _logger.ForContext<MimeDBData>());
        var streamFactory = new StreamFactory(outDir, _logger.ForContext<StreamFactory>());
        var mimeFile = new CompiledFile("Mime.csv", streamFactory, _logger.ForContext<CompiledFile>());
        var mimeIndexFile = new IndexFile("MimeIdx.csv", streamFactory, _logger.ForContext<IndexFile>());
        var extensionFile = new CompiledFile("Extension.csv", streamFactory, _logger.ForContext<CompiledFile>());
        var extensionIndexFile = new IndexFile("ExtensionIdx.csv", streamFactory, _logger.ForContext<IndexFile>());
        var resourceLoader = new ResourceLoader();
        var defaultCsv = new ResourceParser("Default.csv", resourceLoader, _logger.ForContext<ResourceParser>());
        var addendumCsv = new ResourceParser("Addendum.csv", resourceLoader, _logger.ForContext<ResourceParser>());
        _compiler = new Compiler(apacheData,
                                 mimeDBData,
                                 mimeFile,
                                 mimeIndexFile,
                                 extensionFile,
                                 extensionIndexFile,
                                 defaultCsv,
                                 addendumCsv,
                                 _logger.ForContext<Compiler>());
        _readmeFile = new ReadmeFile(outDir, resourceLoader, _logger.ForContext<ReadmeFile>());
    }

    public Compiler ResolveCompiler() => _compiler;

    public ReadmeFile ResolveReadmeFile() => _readmeFile;


    public void Dispose()
    {
        _compiler.Dispose();
        GC.SuppressFinalize(this);
    }
}
