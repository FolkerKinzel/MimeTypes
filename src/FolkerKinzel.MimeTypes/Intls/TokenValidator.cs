using FolkerKinzel.MimeTypes.Properties;

namespace FolkerKinzel.MimeTypes.Intls;

/// <summary>
/// Validates tokens.
/// </summary>
internal static class TokenValidator
{
    /// <summary>
    /// Tests whether a <see cref="string"/> method parameter represents a valid token.
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <param name="paraName">The parameter's name.</param>
    /// <param name="parameterKeyValidation">If <c>true</c>, three aditional characters ('*', '\'', '%') 
    /// are treated as invalid since RFC 2231 doesn't allow these characters in parameter keys.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not a valid token.</exception>
    /// <remarks>
    /// Throws if <paramref name="value"/> is an empty string or consists only of white space. Leading 
    /// and trailing white space around a valid token is accepted.
    /// </remarks>
    internal static void ValidateTokenParameter(this string value,
                                                string? paraName,
                                                bool parameterKeyValidation)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paraName);
        }

        if (!value.AsSpan().IsToken(parameterKeyValidation))
        {
            throw new ArgumentException(string.Format(Res.NotAToken, paraName), paraName);
        }
    }

    /// <summary>
    /// Tests whether <paramref name="value"/> is a valid token.
    /// </summary>
    /// <param name="value">The span to test.</param>
    /// <param name="parameterKeyValidation">If <c>true</c>, three aditional characters ('*', '\'', '%') 
    /// are treated as invalid since RFC 2231 doesn't allow these characters in parameter keys.</param>
    /// <returns><c>true</c> if <paramref name="value"/> is a valid token, otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Returns <c>false</c> even when <paramref name="value"/> is empty or consists only of white space. 
    /// Leading and trailing white space around a valid token is accepted.
    /// </remarks>
    internal static bool IsToken(this ReadOnlySpan<char> value, bool parameterKeyValidation)
    {
        value = value.Trim();

        if (value.IsEmpty)
        {
            return false;
        }

        for (int i = 0; i < value.Length; i++)
        {
            if (!value[i].IsTokenChar(parameterKeyValidation))
            {
                return false;
            }
        }
        return true;
    }

    internal static bool IsTokenChar(this char c, bool parameterKeyValidation)
    {
        // RFC 2231 defines 3 additional characters that are not allowed
        // in a parameter key:
        if (parameterKeyValidation)
        {
            switch (c)
            {
                case '*':
                case '\'':
                case '%':
                    return false;
                default:
                    break;

            }
        }

        switch (c)
        {
            case '\"':
            case '[':
            case '\\':
            case ']':
            case '/':
            case ',':
            case '(':
            case ')':
                return false;
            default:
                break;
        }

        int i = c;
        return i is >= 33 and <= 126 and (<= 57 or >= 65);
    }
}
