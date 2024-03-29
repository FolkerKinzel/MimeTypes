﻿namespace MimeResourceCompiler.Classes;

/// <summary>
/// Represents the compiled file "Extension.csv".
/// </summary>
public sealed class ExtensionFile(IStreamFactory streamFactory, ILogger log) : CompiledFile(streamFactory, log)
{

    /// <summary>
    /// The filename of the compiled file. (Without path information.)
    /// </summary>
    public override string FileName => "Extension.csv";

}
