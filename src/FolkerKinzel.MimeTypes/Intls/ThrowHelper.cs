using FolkerKinzel.MimeTypes.Properties;

namespace FolkerKinzel.MimeTypes.Intls;


internal static class ThrowHelper
{
    internal static void ThrowOnTokenError(TokenError error, string paraName)
    {
        switch (error)
        {
            case TokenError.EmptyString:
                throw new ArgumentException(string.Format(Res.EmptyString, paraName));

            case TokenError.ContainsWhiteSpace:
                throw new ArgumentException(string.Format(Res.ContainsWhiteSpace, paraName), paraName);

            case TokenError.ContainsControl:
                throw new ArgumentException(string.Format(Res.ContainsControlCharacter, paraName), paraName);

            case TokenError.ContainsTSpecial:
                throw new ArgumentException(string.Format(Res.ContainsTSpecial, paraName), paraName);

            case TokenError.ContainsNonAscii:
                throw new ArgumentException(string.Format(Res.ContainsNonAscii, paraName), paraName);

            case TokenError.ContainsReservedCharacter:
                throw new ArgumentException(string.Format(Res.ContainsReservedCharacter, paraName), paraName);

            default:
                break;
        }
    }
}
