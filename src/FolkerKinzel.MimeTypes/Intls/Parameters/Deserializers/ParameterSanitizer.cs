using System.Runtime.InteropServices;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Deserializers;

[StructLayout(LayoutKind.Auto)]
internal ref struct ParameterSanitizer
{
    private const char SEPARATOR = MimeTypeParameter.SEPARATOR;

    private ReadOnlySpan<char> _span;
    private ReadOnlyMemory<char> _parameterString;

    internal bool RepairParameterString(ref ReadOnlyMemory<char> parameterString)
    {
        _parameterString = parameterString;
        _parameterString = _parameterString.Trim();

        if (_parameterString.Length == 0)
        {
            return false;
        }

        int keyLength = UpdateKeyLength();

        if (keyLength < 1)
        {
            return false;
        }

        // If the parameter contains whitespace before the 
        // value part or after the key, repair it:
        keyLength = RemoveWhiteSpaceBetweenKeyAndValue(keyLength);

        // Remove comment at start:
        if (_span[0].Equals('('))
        {
            keyLength = RemoveCommentAtStart();
        }

        if (keyLength < 1) // (Comment)=Value
        {
            return false;
        }

        // Remove comment at End
        // key="value(x\"x)" (Comment)
        if (_span[_span.Length - 1].Equals(')'))
        {
            if (!RemoveCommentAtTheEnd(keyLength + 1))
            {
                return false;
            }
            // Don't use _span after that: It's not
            // up to date!
        }

        parameterString = _parameterString;
        return true;
    }

    private int UpdateKeyLength()
    {
        _span = _parameterString.Span;
        int keyLength = _span.IndexOf(SEPARATOR);
        return keyLength;
    }

    private int RemoveWhiteSpaceBetweenKeyAndValue(int keyValueSeparatorIndex)
    {
        int idxBeforeKeyValueSeparator = keyValueSeparatorIndex - 1;
        int idxAfterKeyValueSeparator = keyValueSeparatorIndex + 1;
        if (_span[idxBeforeKeyValueSeparator].IsWhiteSpace() ||
           (_span.Length > idxAfterKeyValueSeparator && _span[idxAfterKeyValueSeparator].IsWhiteSpace()))
        {
            var sb = new StringBuilder(_span.Length);
            _ = sb.Append(_span.Slice(0, keyValueSeparatorIndex).TrimEnd())
                  .Append('=')
                  .Append(_span.Slice(idxAfterKeyValueSeparator).TrimStart());

            _parameterString = sb.ToString().AsMemory();
            keyValueSeparatorIndex = UpdateKeyLength();
        }
        return keyValueSeparatorIndex;
    }


    private int RemoveCommentAtStart()
    {
        int commentLength = GetCommentLengthAtStart(_span);

        if (commentLength > _span.Length - 2)
        {
            return -1;
        }

        _parameterString = _parameterString.Slice(commentLength + 1).TrimStart();
        return UpdateKeyLength();

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


    /// <summary>
    /// Removes a comment at the end.
    /// </summary>
    /// <param name="valueStart"></param>
    /// <returns><c>true</c> if success.</returns>
    /// <remarks>
    /// IMPORTANT: This method does not update _span!
    /// </remarks>
    private bool RemoveCommentAtTheEnd(int valueStart)
    {
        int commentStartIndex = GetCommentStartIndexAtEnd(_span, valueStart);

        if (commentStartIndex == -1) // ')' at the end of Value
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
