using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace FolkerKinzel.MimeTypes;


public readonly partial struct MimeTypeParameter
{ 
    /// <summary>
    /// Tries to parse a read-only character memory as <see cref="MimeTypeParameter"/>.
    /// </summary>
    /// <param name="firstRun"><c>true</c> if the method runs the first time on <paramref name="parameterString"/>. If the parameter is split 
    /// across multiple lines, the method has to be called twice. Changes to <paramref name="parameterString"/> are only applied in the first run.</param>
    /// <param name="parameterString"></param>
    /// <param name="parameter">When the method returns <c>true</c> the parameter holds the parsed <see cref="MimeTypeParameter"/>.</param>
    /// 
    /// <returns><c>true</c> if <paramref name="parameterString"/> could be parsed as <see cref="MimeTypeParameter"/>.</returns>
    internal static bool TryParse(bool firstRun, ref ReadOnlyMemory<char> parameterString, out MimeTypeParameter parameter)
    {
        parameter = default;

        if (firstRun)
        {
            var sanitizer = new ParameterSanitizer();

            if (!sanitizer.RepairParameterString(ref parameterString))
            {
                return false;
            }
        }

        var idx = new Indexes();

        idx.span = parameterString.Span;

        int keyValueSeparatorIndex = idx.span.IndexOf(SEPARATOR);
        idx.keyLength = idx.span.Slice(0, keyValueSeparatorIndex).GetTrimmedLength();
        if (!idx.VerifyKeyLength())
        {
            return false;
        }
        
        idx.valuePartStart = keyValueSeparatorIndex + 1;

        // A trailing '*' in the Key indicates that charset and/or language might be present (RFC 2184).
        // If the value is in Double-Quotes, no trailing '*' in the Key is allowed.
        if (idx.span[idx.keyLength - 1] == '*')
        {
            --idx.keyLength; // Eat the trailing '*'.
            ++idx.keyValueOffset;

            if (idx.keyLength is 0)
            {
                return false;
            }

            idx.InitCharsetAndLanguage();

            if (firstRun)
            {
                if (!TryDecodeUrl(in idx, ref parameterString))
                {
                    return false;
                }
            }
        }
        else
        {
            // Masked Value:
            // Span cannot end with " when Url encoded because " must be URL encoded then.
            // In the second run parameter.Value cannot be quoted anymore.
            int spanLastIndex = idx.span.Length - 1;
            if (firstRun && idx.span[spanLastIndex] == '\"' && spanLastIndex > idx.valuePartStart && idx.span[idx.valuePartStart] == '\"')
            {
                if (idx.span.Slice(idx.valuePartStart).Contains('\\')) // Masked chars
                {
                    ProcessQuotedAndMaskedValue(idx.valuePartStart, ref parameterString);
                }
                else // No masked chars - tspecials only
                {
                    ProcessQuotedValue(ref parameterString, ref idx.keyValueOffset);
                }
            }
        }

        

        if(!idx.Verify())
        {
            return false;
        }

        int parameterIdx = InitParameterIdx(in idx);

        parameter = new MimeTypeParameter(in parameterString, parameterIdx);
        return true;

        /////////////////////////////////////////////////////////////////////////////////////


        

        static int InitParameterIdx(in Indexes idx)
        {
            int parameterIdx = idx.keyLength;
            parameterIdx |= idx.keyValueOffset << KEY_VALUE_OFFSET_SHIFT;

            if (idx.languageStart != 0)
            {
                parameterIdx |= 1 << CHARSET_LANGUAGE_INDICATOR_SHIFT;
                parameterIdx |= idx.charsetLength << CHARSET_LENGTH_SHIFT;
                parameterIdx |= idx.languageLength << LANGUAGE_LENGTH_SHIFT;
            }

            return parameterIdx;
        }
    }

    private static bool TryDecodeUrl(in Indexes idx, ref ReadOnlyMemory<char> parameterString)
    {
        int valueStart = idx.languageStart + idx.languageLength + 2;
        var valueSpan = idx.span.Slice(valueStart);

        if (valueSpan.Contains('%'))
        {
            var charsetSpan = idx.span.Slice(idx.valuePartStart, idx.charsetLength);
            if (!UrlEncoding.TryDecode(valueSpan.ToString(), charsetSpan, out string? decoded))
            {
                return false;
            }
            var sb = new StringBuilder(valueStart + decoded.Length);
            sb.Append(idx.span.Slice(0, valueStart)).Append(decoded);

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

    private static void ProcessQuotedValue(ref ReadOnlyMemory<char> parameterString, ref int keyValueOffset)
    {
        // Eat the Double-Quotes:
        parameterString = parameterString.Slice(0, parameterString.Length - 1);
        keyValueOffset++;
    }

}
