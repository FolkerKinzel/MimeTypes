using FolkerKinzel.MimeTypes.Intls.Parameters.Creations;
using FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameterInfo
{
    /// <summary>
    /// Tries to parse a read-only character memory as <see cref="MimeTypeParameterInfo"/>.
    /// </summary>
    /// <param name="sanitizeParameterString"><c>true</c> indicates that the 
    /// <paramref name="parameterString"/> should be examinated and sanitized.</param>
    /// <param name="parameterString">The ReadOnlyMemory&lt;char&gt; to be parsed.</param>
    /// <param name="parameterInfo">When the method returns <c>true</c> the parameter holds 
    /// the parsed <see cref="MimeTypeParameterInfo"/>.</param>
    /// <param name="starred">After the method has returned, the argument value indicates 
    /// whether the MIME type parameter's key have had a trailing '*' character that indicates
    /// that language and charset has been present and URL-encoding has been used. This information 
    /// is important because the trailing '*' is eaten by the method.</param>
    /// 
    /// <returns><c>true</c> if <paramref name="parameterString"/> could be parsed as 
    /// <see cref="MimeTypeParameterInfo"/>.</returns>
    internal static bool TryParse(bool sanitizeParameterString,
                                  ref ReadOnlyMemory<char> parameterString,
                                  [NotNull] out MimeTypeParameterInfo parameterInfo,
                                  out bool starred)
    {
        parameterInfo = default;
        starred = false;

        ParameterIndexes idx;

        if (sanitizeParameterString)
        {
            var sanitizer = new ParameterSanitizer();

            if (!sanitizer.RepairParameterString(ref parameterString))
            {
                return false;
            }

            idx = new ParameterIndexes(parameterString.Span);
        }
        else // 2nd run: Splitted parameters have to be parsed twice
        {
            idx = new ParameterIndexes(parameterString.Span);

            Debug.Assert(idx.KeyLength > 0); // with KeyLength == 0 it can't be part of a splitted parameter
            Debug.Assert(!idx.Span.Slice(0, idx.KeyLength).ContainsWhiteSpace()); // removed in the first run
        }

        if (!idx.Verify())
        {
            return false;
        }

        if (idx.LanguageLength != 0 
            && !IetfLanguageTag.Validate(idx.Span.Slice(idx.LanguageStart, idx.LanguageLength)))
        {
            return false;
        }

        if (idx.Decode() && !ParameterValueDecoder.TryDecodeValue(in idx, ref parameterString))
        {
            return false;
        }

        parameterInfo = new MimeTypeParameterInfo(in parameterString, idx.InitMimeTypeParameterCtorIdx());
        starred = idx.Starred;
        return true;
    }
}
