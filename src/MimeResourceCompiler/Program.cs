﻿using CommandLine;
using Serilog.Core;
using Serilog.Events;

namespace MimeResourceCompiler;

/// <summary>
/// Console application, which compiles the resource files needed in the FolkerKinzel.Uris.dll to parse
/// MIME types and file type extensions in a fast way.
/// </summary>
/// <remarks>
/// <para>
/// The main part of the data used to find appropriate file type extensions for MIME types or to find an appropriate
/// MIME type for a file type extension comes from
/// </para>
/// <list type="number">
/// <item>
/// Apache (http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types) 
/// [Change log at: https://svn.apache.org/viewvc/httpd/httpd/trunk/docs/conf/mime.types?view=log].
/// The program loads the current version of this file from the internet each time it runs and enriches these data with the
/// entries from
/// </item>
/// <item>
/// mime-db (https://cdn.jsdelivr.net/gh/jshttp/mime-db@master/db.json) [Change log: https://cdn.jsdelivr.net/gh/jshttp/mime-db@master/HISTORY.md],
/// </item>
/// <item>
/// and the resource file Resources\Addendum.csv. The data in this file is self collected
/// <list type="bullet">
/// <item>
/// at https://wiki.selfhtml.org/wiki/MIME-Type/%C3%9Cbersicht ,
/// </item>
/// <item>
/// at https://mimesniff.spec.whatwg.org/,
/// </item>
/// <item>
/// and from several articles in WIKIPEDIA.
/// </item>
/// </list>
/// </item>
/// </list>
/// <para>
/// The resource file Resources\Addendum.csv allows to add entries that are missing in the Apache file and in mime-db. Entries in Addendum.csv don't produce
/// duplicates even if they are already present in the Apache file or mime-db.
/// </para>
/// <para>
/// The resource file Resources\Default.csv allows to determine the order in which entries are selected. The Apache file is mostly in alphabetical
/// order: That produces not always the expected results. Entries in Resources\Default.csv can never be overwritten by data from external sources.
/// </para>
/// </remarks>
internal class Program
{
    private const string DIRECTORY_NAME = "Mime Resources";
    private const string LOG_FILE_NAME = "_LastBuild.log";

    private static void Main(string[] args)
    {
        _ = Parser.Default.ParseArguments<Options>(args)
                            .WithParsed(options => RunCompiler(options))
                            .WithNotParsed(errs => OnCommandLineParseErrors(errs));
    }

    private static void RunCompiler(Options options)
    {
        string outDir;
        string logFilePath;

        try
        {
            outDir = CreateOutputDirectory(options.OutputPath, options.CreateWrapper);
            logFilePath = Path.Combine(outDir, LOG_FILE_NAME);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Environment.ExitCode = -1;
            return;
        }

        using Logger log = InitializeLogger(options.CreateLogFile ? logFilePath : null, options.LogToConsole);

        try
        {
            using var factory = new Factory(outDir, options, log);

            using (Compiler compiler = factory.ResolveCompiler())
            {
                compiler.CompileResources();
            }

            if (options.CreateReadme)
            {
                factory.ResolveReadmeFile().Create();
            }

            log.Information("Mime resources successfully created at {outDir}.", outDir);
            log.Information("A log file has been created at {logFilePath}.", logFilePath);
        }
        catch (Exception e)
        {
            log.Fatal(e.Message);
            log.Debug(e.ToString());
            Environment.ExitCode = -1;
        }
    }

    private static string CreateOutputDirectory(string rootDirectory, bool createWrapper)
    {
        rootDirectory = Path.GetFullPath(rootDirectory);

        if (createWrapper)
        {
            rootDirectory = Path.Combine(rootDirectory, DIRECTORY_NAME);
        }

        _ = Directory.CreateDirectory(rootDirectory);

        return rootDirectory;
    }

    private static void OnCommandLineParseErrors(IEnumerable<Error> errs)
    {
        foreach (Error err in errs)
        {
            switch (err.Tag)
            {
                case ErrorType.HelpRequestedError:
                case ErrorType.VersionRequestedError:
                    continue;

                default:
                    break;
            }

            Console.Error.WriteLine(err);
            Environment.ExitCode = -1;
        }
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
