namespace MimeResourceCompiler.Classes;

/// <summary>
/// ctor
/// </summary>
/// <param name="resourceLoader">IResourceLoader</param>
/// <param name="log">ILogger</param>
public sealed class DefaultEntry(IResourceLoader resourceLoader, ILogger log) : ResourceParser(resourceLoader, log)
{
    public override string FileName => "Default.csv";

}
