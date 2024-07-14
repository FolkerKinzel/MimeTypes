using CommandLine;

namespace MimeResourceCompiler;

/// <summary>
/// Encapsulates the command line options.
/// </summary>
public class Options(string outputPath, bool createWrapper, bool createReadme, bool createLogFile, bool logToConsole)
{
    private readonly string? _outputPath = outputPath;
    private readonly bool _createReadme = createReadme;
    private readonly bool _createLogFile = createLogFile;
    private readonly bool _logToConsole = logToConsole;
    private readonly bool _createWrapper = createWrapper;

    [Option('p', "path", Required = false, HelpText = "Path to the directory that gets the compiled output.")]
    public string OutputPath => _outputPath ?? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

    [Option('w', "wrapper", Required = false, Default = true, HelpText = "Set the argument to not wrap the output in a separate directory.")]
    public bool CreateWrapper => _createWrapper;

    [Option('r', "readme", Required = false, Default = true, HelpText = "Set the argument to create no Readme.txt file.")]
    public bool CreateReadme => _createReadme;

    [Option('l', "logfile", Required = false, Default = false, HelpText = "Set the argument to create a log file.")]
    public bool CreateLogFile => _createLogFile;

    [Option('c', "consolelog", Required = false, Default = false, HelpText = "Set the argument to write debug logging to the console.")]
    public bool LogToConsole => _logToConsole;
}
