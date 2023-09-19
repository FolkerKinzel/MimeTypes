namespace FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

internal static class IetfLanguageTag
{
    internal static bool Validate(string? language)
    {
        if (string.IsNullOrWhiteSpace(language) || language.Length > MimeTypeParameterInfo.LANGUAGE_LENGTH_MAX_VALUE)
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
