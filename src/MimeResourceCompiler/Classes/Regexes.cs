using System.Text.RegularExpressions;

namespace MimeResourceCompiler.Classes;

public static partial class Regexes
{
    [GeneratedRegex(@"\s+")]
    public static partial Regex WhiteSpace();
}
