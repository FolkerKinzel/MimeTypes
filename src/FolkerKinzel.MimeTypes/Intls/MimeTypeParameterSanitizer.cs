using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System.Runtime.InteropServices;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls;

[StructLayout(LayoutKind.Auto)]
internal ref struct MimeTypeParameterSanitizer
{
    private const char SEPARATOR = MimeTypeParameter.SEPARATOR;

    private ReadOnlySpan<char> _span;
    private ReadOnlyMemory<char> _parameterString;


    internal bool RepairParameterString(ref ReadOnlyMemory<char> parameterString)
    {
        _parameterString = parameterString;
        _parameterString = _parameterString.Trim();
        _span = _parameterString.Span;


        if (_parameterString.Length == 0)
        {
            goto Fail;
        }

        int keyValueSeparatorIndex = _span.IndexOf(SEPARATOR);

        if (keyValueSeparatorIndex < 1)
        {
            goto Fail;
        }

        // If the parameter contains whitespace before the 
        // value part or after the key, repair it:
        keyValueSeparatorIndex = RemoveWhiteSpaceBetweenKeyAndValue(keyValueSeparatorIndex);


        // Remove comment at start:
        if (_span[0].Equals('('))
        {
            RemoveCommentAtStart();
            keyValueSeparatorIndex = _span.IndexOf(SEPARATOR);
        }

        int valueStart = keyValueSeparatorIndex + 1;

        if (valueStart == 0) // '=' in the comment
        {
            goto Fail;
        }

        // Remove comment at End
        // key="value(x\"x)" (Comment)
        if (_span[_span.Length - 1].Equals(')'))
        {
            if (!RemoveCommentAtTheEnd(valueStart))
            {
                goto Fail;
            }
        }

        parameterString = _parameterString;
        return true;

    Fail:
        parameterString = default;
        return false;
    }


    private int RemoveWhiteSpaceBetweenKeyAndValue(int keyValueSeparatorIndex)
    {
        int idxBeforeKeyValueSeparator = keyValueSeparatorIndex - 1;
        int idxAfterKeyValueSeparator = keyValueSeparatorIndex + 1;
        if (_span[idxBeforeKeyValueSeparator].IsWhiteSpace() ||
           _span.Length > idxAfterKeyValueSeparator && _span[idxAfterKeyValueSeparator].IsWhiteSpace())
        {
            var sb = new StringBuilder(_span.Length);
            _ = sb.Append(_span.Slice(0, keyValueSeparatorIndex).TrimEnd())
                  .Append('=')
                  .Append(_span.Slice(idxAfterKeyValueSeparator).TrimStart());

            _parameterString = sb.ToString().AsMemory();
            _span = _parameterString.Span;
            keyValueSeparatorIndex = _span.IndexOf(SEPARATOR);
        }
        return keyValueSeparatorIndex;
    }


    private void RemoveCommentAtStart()
    {
        int commentLength = GetCommentLengthAtStart(_span);
        _parameterString = _parameterString.Slice(commentLength + 1).TrimStart();
        _span = _parameterString.Span;

        ///////////////////////////////////////////////////////////

        static int GetCommentLengthAtStart(ReadOnlySpan<char> span)
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
    }


    private bool RemoveCommentAtTheEnd(int valueStart)
    {
        int commentStartIndex = GetCommentStartIndexAtEnd(_span, valueStart);

        if (commentStartIndex == -1)
        {
            return false;
        }

        _parameterString = _parameterString.Slice(0, commentStartIndex).TrimEnd();
        return true;

        ///////////////////////////////////////////////////////////////

        static int GetCommentStartIndexAtEnd(ReadOnlySpan<char> span, int valueStart)
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
    }
}
