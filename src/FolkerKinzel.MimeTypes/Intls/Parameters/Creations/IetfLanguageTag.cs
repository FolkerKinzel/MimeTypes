namespace FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

internal static class IetfLanguageTag
{
    internal static bool Validate(ReadOnlySpan<char> language)
    {
        if (language.IsEmpty)
        {
            return false;
        }

        for (int i = 0; i < language.Length; i++)
        {
            char current = language[i];

            if (!(current.IsAsciiLetterOrDigit() || current == '-'))
            {
                return false;
            }
        }
        return true;
    }
}
