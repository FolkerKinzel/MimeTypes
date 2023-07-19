using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class ParameterSplitter
{
    internal static bool IsParameterSplitted(this ReadOnlySpan<char> keySpan) => keySpan.GetStarIndex() != -1;

    internal static int GetStarIndex(this ReadOnlySpan<char> keySpan)
    {
        int idx = keySpan.Length - 2;

        while (idx > 0)
        {
            char c = keySpan[idx];
            if (c == '*')
            {
                return idx;
            }
            else if (!c.IsDecimalDigit())
            {
                return -1;
            }

            idx--;
        }

        return -1;
    }


    private const int DOUBLE_QUOTES_COUNT = 2;

    /// <summary>
    /// Splits a long <see cref="MimeTypeParameter"/> into several parts and returns them as a collection of <see cref="StringBuilder"/>
    /// objects.
    /// </summary>
    /// <param name="parameter">The <see cref="MimeTypeParameter"/> to split.</param>
    /// <param name="worker">A <see cref="StringBuilder"/> that holds the serialized <paramref name="parameter"/>.</param>
    /// <param name="lineLength">The line length at which the parameter should be splitted.</param>
    /// <returns>A collection of <see cref="StringBuilder"/> objects that represents the splitted <paramref name="parameter"/>.</returns>
    internal static IEnumerable<StringBuilder> SplitParameter(MimeTypeParameter parameter, StringBuilder worker, int lineLength)
    {
        Debug.Assert(worker.Length > 0);
        const int COUNTER_INITIAL_LENGTH = 1;

        bool quoted = worker[worker.Length - 1] == '"';
        bool urlEncoded = !quoted && (!parameter.Language.IsEmpty || worker.Contains('%'));

        PrepareWorker(worker, quoted, urlEncoded);
        int minimumLength = ComputeMinimumLength(parameter, quoted, urlEncoded);

        if (lineLength < minimumLength)
        {
            lineLength = minimumLength;
        }

        var tmp = new StringBuilder(lineLength);
        _ = tmp.Append(parameter.Key).Append('*');

        int counterIdx = tmp.Length;
        int counter = 0;
        _ = tmp.Append(counter);

        if (urlEncoded)
        {
            _ = tmp.Append('*');
        }
        _ = tmp.Append('=');

        int normalValueStart = tmp.Length - COUNTER_INITIAL_LENGTH;

        if (urlEncoded)
        {
            _ = tmp.Append(ParameterSerializer.UTF_8).Append('\'').Append(parameter.Language).Append('\'');
        }

        int valueStart = tmp.Length;
        int valLength;
        do
        {
            valLength = lineLength - valueStart;

            if (quoted)
            {
                valLength -= DOUBLE_QUOTES_COUNT;
            }

            // The rest of the last line:
            if (valLength > worker.Length)
            {
                valLength = worker.Length;
            }

            if (quoted)
            {
                tmp.Append('\"');
            }

            _ = tmp.Append(worker, 0, valLength);
            _ = worker.Remove(0, valLength);

            if (quoted)
            {
                tmp.Append('\"');
            }

            yield return tmp;

            if(worker.Length == 0)
            {
                yield break;
            }

            RemoveCurrentValue(tmp, counter, normalValueStart);
            UpdateCounter(tmp, counterIdx, ref counter);

            valueStart = normalValueStart + counter.DigitsCount();

        } while (valLength > 5); // Security: if counter gets too large valLength could become negative
    }

    private static void RemoveCurrentValue(StringBuilder tmp, int counter, int valueStart)
    {
        valueStart += counter.DigitsCount();
        _ = tmp.Remove(valueStart, tmp.Length - valueStart);
    }

    private static void UpdateCounter(StringBuilder tmp, int counterIdx, ref int counter)
    {
        tmp.Remove(counterIdx, counter.DigitsCount());
        int i = ++counter;
        do
        {
            tmp.Insert(counterIdx, (char)((i % 10) + '0'));
            i /= 10;
        }
        while(i != 0);
    }


    /// <summary>
    /// Removes anything from worker except the parameter value.
    /// </summary>
    /// <param name="worker"></param>
    /// <param name="quoted"></param>
    /// <param name="urlEncoded"></param>
    private static void PrepareWorker(StringBuilder worker, bool quoted, bool urlEncoded)
    {
        int startOfValue = worker.IndexOf('=') + 1;
        if (quoted)
        {
            startOfValue++; // ="
            _ = worker.Remove(worker.Length - 1, 1);
        }
        else if (urlEncoded)
        {
            startOfValue = worker.IndexOf('\'', startOfValue);
            startOfValue = worker.IndexOf('\'', startOfValue + 1) + 1;
        }
        _ = worker.Remove(0, startOfValue);
    }

    /// <summary>
    /// Computes the minimum length that is needed for a line. (Depends on key, charset,
    /// language and quotes.)
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="quoted"></param>
    /// <param name="urlEncoded"></param>
    /// <returns>The minimum length that is needed for a line. </returns>
    private static int ComputeMinimumLength(MimeTypeParameter parameter, bool quoted, bool urlEncoded)
    {
        int minimumLength = parameter.Key.Length + 9; // *0= and 6 value chars per line at least

        if (quoted)
        {
            minimumLength += 2; // ""
        }
        else if (urlEncoded)
        {
            minimumLength += 3 + parameter.CharSet.Length + parameter.Language.Length; // *''
        }

        return minimumLength;
    }
}
