namespace MimeResourceCompiler.Classes;

/// <summary>
/// Represents the content of Default.csv
/// </summary>
/// <param name="resourceLoader">IResourceLoader</param>
/// <param name="log">ILogger</param>
public sealed class DefaultEntry(IResourceLoader resourceLoader, ILogger log) : ResourceParser(resourceLoader, log)
{
    public override string FileName => "Default.csv";
}
