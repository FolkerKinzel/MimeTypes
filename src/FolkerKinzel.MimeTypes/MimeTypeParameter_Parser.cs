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
    internal const char SEPARATOR = '=';

    /// <summary>
    /// Tries to parse a read-only character memory as <see cref="MimeTypeParameter"/>.
    /// </summary>
    /// <param name="firstRun"><c>true</c> if the method runs the first time on <paramref name="parameterString"/>. If the parameter is split 
    /// across multiple lines, the method has to be called twice. Changes to <paramref name="parameterString"/> are only applied in the first run.</param>
    /// <param name="parameterString"></param>
    /// <param name="parameter">When the method returns <c>true</c> the parameter holds the parsed <see cref="MimeTypeParameter"/>.</param>
    /// <param name="quoted">When the method returns, indicates whether the parameter value is in double quotes. On the first run this is important to
    /// know to indicate whether Url encoding should be removed or not. <see cref="MimeType.ParseParameters"/></param>
    /// <returns><c>true</c> if <paramref name="parameterString"/> could be parsed as <see cref="MimeTypeParameter"/>.</returns>
    internal static bool TryParse(bool firstRun, ref ReadOnlyMemory<char> parameterString, out MimeTypeParameter parameter, out bool quoted)
    {
        quoted = false;
        parameter = default;


        if (firstRun)
        {
            var sanitizer = new MimeTypeParameterSanitizer();
        
            if(!sanitizer.RepairParameterString(ref parameterString))
            {
                return false;
            }
        }

        ReadOnlySpan<char> span = parameterString.Span;

        int keyValueSeparatorIndex = span.IndexOf(SEPARATOR);
        int valueStart = keyValueSeparatorIndex + 1;

        int keyLength = span.Slice(0, keyValueSeparatorIndex).GetTrimmedLength();

        if (keyLength is 0 or > KEY_LENGTH_MAX_VALUE)
        {
            return false;
        }

        int languageStart = 0;
        int languageLength = 0;
        int charsetLength = 0;
        int keyValueOffset = 0;

        // A trailing '*' in the Key indicates that charset and/or language might be present (RFC 2184).
        // If the value is in Double-Quotes, no trailing '*' in the Key is allowed.
        if (span[keyLength - 1] == '*')
        {
            --keyLength; // Eat the trailing '*'.
            ++keyValueOffset;

            if (keyLength is 0)
            {
                return false;
            }

            InitCharsetAndLanguageIdx(span, valueStart, ref charsetLength, ref languageStart, ref languageLength);
        }
        else
        {
            // Masked Value:
            // Span cannot end with " when Url encoded because " must be URL encoded then.
            // In the second run parameter.Value cannot be quoted anymore.
            int spanLastIndex = span.Length - 1;
            if (firstRun && span[spanLastIndex] == '\"' && spanLastIndex > valueStart && span[valueStart] == '\"')
            {
                quoted = true;
                if (span.Slice(valueStart).Contains('\\')) // Masked chars
                {
                    ProcessQuotedAndMaskedValue(valueStart, ref parameterString);
                }
                else // No masked chars - tspecials only
                {
                    ProcessQuotedValue(ref parameterString, ref keyValueOffset);
                }
            }
        }

        if (keyValueOffset > KEY_VALUE_OFFSET_MAX_VALUE || charsetLength > CHARSET_LENGTH_MAX_VALUE || languageLength > LANGUAGE_LENGTH_MAX_VALUE)
        {
            return false;
        }

        int idx = InitIdx(keyLength, keyValueOffset, charsetLength, languageStart, languageLength);

        parameter = new MimeTypeParameter(in parameterString, idx);
        return true;


        static void ProcessQuotedAndMaskedValue(int valueStart, ref ReadOnlyMemory<char> parameterString)
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

        static void ProcessQuotedValue(ref ReadOnlyMemory<char> parameterString, ref int keyValueOffset)
        {
            // Eat the Double-Quotes:
            parameterString = parameterString.Slice(0, parameterString.Length - 1);
            keyValueOffset++;
        }

        

        static void InitCharsetAndLanguageIdx(ReadOnlySpan<char> span, int valueStart, ref int charsetLength, ref int languageStart, ref int languageLength)
        {
            bool startInitialized = false;
            for (int i = valueStart; i < span.Length; i++)
            {
                char c = span[i];

                if (c == '\'')
                {
                    if (!startInitialized)
                    {
                        startInitialized = true;
                        charsetLength = i - valueStart;
                        languageStart = i + 1;
                    }
                    else
                    {
                        languageLength = i - languageStart;
                        break;
                    }
                }
            }
        }


        static int InitIdx(int keyLength, int keyValueOffset, int charsetLength, int languageStart, int languageLength)
        {
            int idx = keyLength;
            idx |= keyValueOffset << KEY_VALUE_OFFSET_SHIFT;

            if (languageStart != 0)
            {
                idx |= 1 << CHARSET_LANGUAGE_INDICATOR_SHIFT;
                idx |= charsetLength << CHARSET_LENGTH_SHIFT;
                idx |= languageLength << LANGUAGE_LENGTH_SHIFT;
            }

            return idx;
        }
    }


    

    


    
}
