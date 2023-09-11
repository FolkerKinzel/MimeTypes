namespace FolkerKinzel.MimeTypes.Intls;

internal static class StringBuilderExtension
{
    internal static StringBuilder Replace( this StringBuilder builder, string oldValue, string? newValue, int startIndex )
        => builder.Replace(oldValue, newValue, startIndex, builder.Length - startIndex);

}

