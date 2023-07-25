namespace FolkerKinzel.MimeTypes.Intls;

internal static class HelperExtension
{
    internal static int DigitsCount(this int input) =>
        input == 0 ? 1 : (int)Math.Floor(Math.Log10(input) + 1);


    internal static IEnumerable<MimeTypeParameterInfo> Sort(this IEnumerable<MimeTypeParameterInfo> parameters, bool isTextMimeType)
    {
        List<MimeTypeParameterInfo>? list = null;

        foreach (MimeTypeParameterInfo parameter in parameters)
        {
            if (isTextMimeType && parameter.IsAsciiCharSetParameter)
            {
                continue;
            }

            list ??= new List<MimeTypeParameterInfo>(2);
            list.Add(parameter);
        }

        if (list is null)
        {
            return Array.Empty<MimeTypeParameterInfo>();
        }

        if (list.Count == 1)
        {
            return list;
        }

        list.Sort();

        return list;
    }


    internal static IEnumerable<MimeTypeParameter> Sort(this IEnumerable<MimeTypeParameter> parameters, bool isTextMimeType)
    {
        List<MimeTypeParameter>? list = null;

        foreach (MimeTypeParameter parameter in parameters)
        {
            if (isTextMimeType && parameter.IsAsciiCharSetParameter)
            {
                continue;
            }

            list ??= new List<MimeTypeParameter>(2);
            list.Add(parameter);
        }

        if (list is null)
        {
            return Array.Empty<MimeTypeParameter>();
        }

        if (list.Count == 1)
        {
            return list;
        }

        list.Sort();

        return list;
    }

}
