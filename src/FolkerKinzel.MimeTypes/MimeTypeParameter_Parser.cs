using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System.Text;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameter
{
    private const char SEPARATOR = '=';

    internal static bool TryParse(bool firstRun, ref ReadOnlyMemory<char> parameterString, out MimeTypeParameter parameter, out bool quoted)
    {
        quoted = false;

        parameterString = parameterString.Trim();

        if (parameterString.Length == 0)
        {
            goto Failed;
        }

        ReadOnlySpan<char> span = parameterString.Span;

        int keyValueSeparatorIndex = span.IndexOf(SEPARATOR);

        if (keyValueSeparatorIndex < 1)
        {
            goto Failed;
        }

        // If the parameter contains whitespace before the 
        // value part or after the key, repair it:
        int idxBeforeKeyValueSeparator = keyValueSeparatorIndex - 1;
        int idxAfterKeyValueSeparator = keyValueSeparatorIndex + 1;
        if (span[idxBeforeKeyValueSeparator].IsWhiteSpace() || 
           (span.Length > idxAfterKeyValueSeparator && span[idxAfterKeyValueSeparator].IsWhiteSpace()))
        {
            var sb = new StringBuilder(span.Length);
            _ = sb.Append(span.Slice(0, keyValueSeparatorIndex).TrimEnd())
                  .Append('=')
                  .Append(span.Slice(idxAfterKeyValueSeparator).TrimStart());

            parameterString = sb.ToString().AsMemory();
            span = parameterString.Span;
            keyValueSeparatorIndex = span.IndexOf(SEPARATOR);
        }

        // Remove comment at start:
        if (span[0].Equals('('))
        {
            int commentLength = GetCommentLengthAtStart(span);
            parameterString = parameterString.Slice(commentLength + 1).TrimStart();
            span = parameterString.Span;

            if (parameterString.Length == 0)
            {
                goto Failed;
            }

            keyValueSeparatorIndex = span.IndexOf(SEPARATOR);
        }

        int valueStart = keyValueSeparatorIndex + 1;


        // Remove comment at End
        // key="value(x\"x)" (Comment)
        if (span[span.Length - 1].Equals(')'))
        {
            int commentStartIndex = GetCommentStartIndexAtEnd(span, valueStart);

            if (commentStartIndex == -1)
            {
                goto Failed;
            }

            parameterString = parameterString.Slice(0, commentStartIndex).TrimEnd();
            span = parameterString.Span;
        }

        int keyLength = span.Slice(0, keyValueSeparatorIndex).GetTrimmedLength();

        if (keyLength is 0 or > KEY_LENGTH_MAX_VALUE)
        {
            goto Failed;
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
                goto Failed;
            }

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
                        valueStart = i + 1;
                        break;
                    }
                }
            }
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
                    var builder = new StringBuilder(parameterString.Length);
                    _ = builder.Append(parameterString).Remove(builder.Length - 1, 1);

                    if (valueStart < builder.Length && builder[valueStart] == '"')
                    {
                        _ = builder.Remove(valueStart, 1);
                        UnMask(builder, valueStart);
                    }

                    ReadOnlyMemory<char> mem = builder.ToString().AsMemory();
                    return TryParse(false, ref mem, out parameter, out _);
                }
                else // No masked chars - tspecials only
                {
                    // Eat the Double-Quotes:
                    parameterString = parameterString.Slice(0, parameterString.Length - 1);
                    keyValueOffset++;
                }
            }
        }

        if (keyValueOffset > KEY_VALUE_OFFSET_MAX_VALUE)
        {
            goto Failed;
        }

        int idx = keyLength;
        idx |= keyValueOffset << KEY_VALUE_OFFSET_SHIFT;

        if (languageStart != 0)
        {
            if (charsetLength > CHARSET_LENGTH_MAX_VALUE ||
                languageLength > LANGUAGE_LENGTH_MAX_VALUE)
            {
                goto Failed;
            }

            idx |= 1 << CHARSET_LANGUAGE_INDICATOR_SHIFT;
            idx |= charsetLength << CHARSET_LENGTH_SHIFT;
            idx |= languageLength << LANGUAGE_LENGTH_SHIFT;
        }


        parameter = new MimeTypeParameter(in parameterString, idx);

        return true;
    ///////////////////////////////
    Failed:
        parameter = default;
        return false;
    }


    private static int GetCommentStartIndexAtEnd(ReadOnlySpan<char> span, int valueStart)
    {
        bool quoted = false;
        for (int i = valueStart; i < span.Length; i++)
        {
            char current = span[i];
            if (current.Equals('\\'))
            {
                i++;
                continue;
            }

            if (current.Equals('\"'))
            {
                quoted = !quoted;
            }

            if (quoted)
            {
                continue;
            }

            if (current.Equals('('))
            {
                return i;
            }
        }

        return -1;
    }

    private static int GetCommentLengthAtStart(ReadOnlySpan<char> span)
    {
        for (int i = 1; i < span.Length; i++)
        {
            char current = span[i];
            if (current.Equals('\\'))
            {
                i++;
                continue;
            }

            if (current.Equals(')'))
            {
                return i;
            }
        }

        return span.Length;
    }


    private static void UnMask(StringBuilder builder, int startOfValue)
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
