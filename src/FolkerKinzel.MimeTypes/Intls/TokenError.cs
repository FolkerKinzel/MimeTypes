namespace FolkerKinzel.MimeTypes.Intls
{
    internal enum TokenError
    {
        None,
        EmptyString,
        ContainsWhiteSpace,
        ContainsControl,
        ContainsTSpecial,
        ContainsNonAscii,
        ContainsReservedCharacter
    }
}
