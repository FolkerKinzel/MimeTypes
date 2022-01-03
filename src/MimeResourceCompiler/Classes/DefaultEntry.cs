namespace MimeResourceCompiler.Classes;

public sealed class DefaultEntry : ResourceParser
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="resourceLoader">IResourceLoader</param>
    /// <param name="log">ILogger</param>
    public DefaultEntry(IResourceLoader resourceLoader, ILogger log) : base(resourceLoader, log) { }

    public override string FileName => "Default.csv";

}
