using System.Text.Json.Serialization;

namespace MimeResourceCompiler.Classes;

public sealed class MimeDBEntry
{
    public string? MimeType { get; set; }

    public string[]? Extensions { get; set; }

    public void Clear()
    {
        MimeType = null;
        Extensions = null;
    }
}
