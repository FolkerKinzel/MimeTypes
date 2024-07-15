using MimeResourceCompiler.Classes;
using Serilog.Core;
using Serilog.Events;

namespace MimeResourceCompiler;

/// <summary>
/// Builds the objects used in the program and takes care to release their resources.
/// </summary>
internal sealed class Factory : IDisposable
{
    private const string LOG_FILE_NAME = "_LastBuild.log";

    private static readonly HttpClient _httpClient = new();

    private readonly Compiler _compiler;
    private readonly ReadmeFile _readmeFile;
    private readonly Logger _logger;

    public Factory(string outDir, Options options)
    {
        //throw new ArgumentNullException("name");
        LogFilePath = Path.Combine(outDir, LOG_FILE_NAME);
        _logger = InitializeLogger(options.CreateLogFile ? LogFilePath : null, options.LogToConsole);

        var apacheData = new ApacheData(_httpClient, _logger.ForContext<ApacheData>());
        var mimeDBData = new MimeDBData(_httpClient, _logger.ForContext<MimeDBData>());
        var streamFactory = new StreamFactory(outDir, _logger.ForContext<StreamFactory>());
        var mimeFile = new MimeFile(streamFactory, _logger.ForContext<MimeFile>());
        var indexFile = new IndexFile(streamFactory, _logger.ForContext<IndexFile>());
        var extensionFile = new ExtensionFile(streamFactory, _logger.ForContext<ExtensionFile>());
        var resourceLoader = new ResourceLoader();
        var defaultEntry = new DefaultEntry(resourceLoader, _logger.ForContext<Addendum>());
        var addendum = new Addendum(resourceLoader, _logger.ForContext<Addendum>());
        var compressor = new Compressor(_logger.ForContext<Compressor>());
        _compiler = new Compiler(apacheData, mimeDBData, mimeFile, indexFile, extensionFile, defaultEntry, addendum, _logger.ForContext<Compiler>(), compressor);
        _readmeFile = new ReadmeFile(outDir, resourceLoader, _logger.ForContext<ReadmeFile>());
    }

    public string LogFilePath { get; }

    public ILogger ResolveLogger() => _logger;

    public Compiler ResolveCompiler() => _compiler;

    public ReadmeFile ResolveReadmeFile() => _readmeFile;


    public void Dispose()
    {
        _compiler.Dispose();
        _logger.Dispose();
        GC.SuppressFinalize(this);
    }

    private static Logger InitializeLogger(string? logFilePath, bool logToConsole)
    {
        LogEventLevel consoleLogEventLevel = logToConsole ? LogEventLevel.Debug : LogEventLevel.Information;

        LoggerConfiguration config = new LoggerConfiguration()
                                    .MinimumLevel.Debug()
                                    .WriteTo.Console(restrictedToMinimumLevel: consoleLogEventLevel);

        if (logFilePath is not null)
        {
            if (File.Exists(logFilePath))
            {
                try
                {
                    File.Delete(logFilePath);
                }
                catch
                { }
            }

            _ = config.WriteTo.File(logFilePath);
        }

        return config.CreateLogger();
    }


}
