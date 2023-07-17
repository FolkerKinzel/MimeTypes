using FolkerKinzel.Strings;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class IetfLanguageTag
{
    internal static bool Validate(string? language)
    {
        if (string.IsNullOrWhiteSpace(language) || language.Length > MimeTypeParameter.LANGUAGE_LENGTH_MAX_VALUE)
        {
            return false;
        }

        for (int i = 0; i < language.Length; i++)
        {
            char current = language[i];

            if (!(char.IsLetter(current) || current == '-') || !current.IsAscii())
            {
                return false;
            }
        }
        return true;
    }
}
