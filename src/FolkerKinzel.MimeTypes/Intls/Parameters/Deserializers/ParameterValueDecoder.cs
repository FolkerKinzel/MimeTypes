using FolkerKinzel.MimeTypes.Intls.Parameters.Encodings;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers;

/// <summary>
/// Helper class that supports the parsing of <see cref="MimeTypeParameterInfo"/> objects
/// in bringing URL encoded or masked or quoted parameter values in a readable form.
/// </summary>
internal static class ParameterValueDecoder
{
    /// <summary>
    /// Removes characters from <paramref name="parameterString"/> that are not thought for human
    /// reading.
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="parameterString">The <see cref="char"/> memory to change.</param>
    /// <returns><c>true</c> if successful, otherwise <c>false</c>.</returns>
    /// <remarks>
    /// <note type="important">
    /// This method may only be called once on <paramref name="parameterString"/> !
    /// </note>
    /// </remarks>
    internal static bool TryDecodeValue(in ParameterIndexes idx, ref ReadOnlyMemory<char> parameterString)
    {
        // A trailing '*' in the Key indicates that charset and/or language might be present (RFC 2231).
        // If the value is in Double-Quotes, no trailing '*' in the Key is allowed.
        if (idx.Starred && !TryDecodeUrl(in idx, ref parameterString))
        {
            return false;
        }
        else if (idx.IsValueQuoted)
        {
            // Quoted Value:
            // Span cannot end with " when Url encoded because " must be URL encoded then.
            // In the second run parameter.Value cannot be quoted anymore.
            if (idx.Span.Slice(idx.ValuePartStart).Contains('\\')) // Masked chars
            {
                ProcessQuotedAndMaskedValue(idx.ValuePartStart, ref parameterString);
            }
            else // No masked chars - tspecials only
            {
                // Eat the trailing Double-Quotes (the leading double quotes are eaten by PramameterIndexes:
                parameterString = parameterString.Slice(0, parameterString.Length - 1);
            }
        }

        // If the the value is not quoted and the key is not starred (key*) the value shall remain as it is
        // (RFC 2231).

        return true;
    }


    private static bool TryDecodeUrl(in ParameterIndexes idx, ref ReadOnlyMemory<char> parameterString)
    {
        int valueStart = idx.ValueStart;
        var valueSpan = idx.Span.Slice(valueStart);

        if (valueSpan.Contains('%'))
        {
            var charsetSpan = idx.Span.Slice(idx.ValuePartStart, idx.CharsetLength);
            if (!UrlEncoding.TryDecode(valueSpan.ToString(), charsetSpan, out string? decoded))
            {
                return false;
            }
            var sb = new StringBuilder(valueStart + decoded.Length);
            sb.Append(idx.Span.Slice(0, valueStart)).Append(decoded);

            parameterString = sb.ToString().AsMemory();
        }
        return true;
    }


    private static void ProcessQuotedAndMaskedValue(int valueStart, ref ReadOnlyMemory<char> parameterString)
    {
        var builder = new StringBuilder(parameterString.Length);

        // Remove the trailing double quote. The leading '"' MUST remain in
        // the builder it is eaten by the indexes
        _ = builder.Append(parameterString).Remove(builder.Length - 1, 1);

        builder.UnMask(valueStart);
        parameterString = builder.ToString().AsMemory();
    }
}
