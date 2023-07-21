namespace FolkerKinzel.MimeTypes.Intls;


[Flags]
internal enum EncodingAction
{
    None = 0,
    Quote = 1,
    Mask = 3,
    UrlEncode = 4
}
