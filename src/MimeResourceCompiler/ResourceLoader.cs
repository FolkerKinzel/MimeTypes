using System.Reflection;

namespace MimeResourceCompiler;

/// <summary>
/// Encapsulates functionality to load the resources.
/// </summary>
public interface IResourceLoader
{
    /// <summary>
    /// Returns a stream to a resource.
    /// </summary>
    /// <param name="fileName">The name of the resource file.</param>
    /// <returns>A stream to the resource.</returns>
    Stream GetResourceStream(string fileName);

}

/// <summary>
/// Encapsulates functionality to load the resources.
/// </summary>
public class ResourceLoader : IResourceLoader
{
    private const string RESOURCE_PATH = "MimeResourceCompiler.Resources.";

    /// <summary>
    /// Returns a stream to a resource.
    /// </summary>
    /// <param name="fileName">The name of the resource file.</param>
    /// <returns>A stream from the resource.</returns>
    public Stream GetResourceStream(string fileName)
    {
        Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_PATH + fileName);
        return stream is null ? throw new InvalidDataException($"The resource {fileName} has not been found.") : stream;
    }
}
