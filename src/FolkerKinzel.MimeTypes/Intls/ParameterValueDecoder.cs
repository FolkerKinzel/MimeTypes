using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls;

/// <summary>
/// Helper class that supports the parsing of <see cref="MimeTypeParameter"/> objects
/// in bringing URL encoded or masked or quoted parameter values in a readable form.
/// </summary>
internal static class ParameterValueDecoder
{
    internal static bool TryDecodeValue(bool firstRun, ref ParameterIndexes idx, ref ReadOnlyMemory<char> parameterString)
    {
        // A trailing '*' in the Key indicates that charset and/or language might be present (RFC 2184).
        // If the value is in Double-Quotes, no trailing '*' in the Key is allowed.
        if (idx.IsValueUrlEncoded())
        {
            InitUrlEncodedOffsets(ref idx);

            if (firstRun && !TryDecodeUrl(in idx, ref parameterString))
            {
                return false;
            }
        }
        else if (firstRun && idx.IsValueQuoted())
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
                ProcessQuotedValue(ref parameterString, ref idx.KeyValueOffset);
            }
        }

        return true;
    }

    private static bool TryDecodeUrl(in ParameterIndexes idx, ref ReadOnlyMemory<char> parameterString)
    {
        int valueStart = idx.LanguageStart + idx.LanguageLength + 2;
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
        _ = builder.Append(parameterString).Remove(builder.Length - 1, 1);

        if (valueStart < builder.Length && builder[valueStart] == '"')
        {
            _ = builder.Remove(valueStart, 1);
            UnMask(builder, valueStart);
        }

        parameterString = builder.ToString().AsMemory();

        //////////////////////////////////////////////

        static void UnMask(StringBuilder builder, int startOfValue)
        {
            for (int i = startOfValue; i < builder.Length; i++)
            {
                if (builder[i] == '\\')
                {
                    // after the mask char one entry can be skipped:
                    _ = builder.Remove(i, 1);
                }
            }
        }
    }


    private static void InitUrlEncodedOffsets(ref ParameterIndexes idx)
    {
        --idx.KeyLength; // Eat the trailing '*'.
        ++idx.KeyValueOffset;
        InitCharsetAndLanguage(ref idx);

        static void InitCharsetAndLanguage(ref ParameterIndexes idx)
        {
            bool inLanguage = false;
            for (int i = idx.ValuePartStart; i < idx.Span.Length; i++)
            {
                char c = idx.Span[i];

                if (c == '\'')
                {
                    if (!inLanguage)
                    {
                        idx.CharsetLength = i - idx.ValuePartStart;
                        idx.LanguageStart = i + 1;
                    }
                    else
                    {
                        idx.LanguageLength = i - idx.LanguageStart;
                        break;
                    }

                    inLanguage = !inLanguage;
                }
            }
        }
    }

    private static void ProcessQuotedValue(ref ReadOnlyMemory<char> parameterString, ref int keyValueOffset)
    {
        // Eat the Double-Quotes:
        parameterString = parameterString.Slice(0, parameterString.Length - 1);
        keyValueOffset++;
    }
}
