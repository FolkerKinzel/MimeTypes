using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

internal static class ParameterSplitter
{
    private const int DOUBLE_QUOTES_COUNT = 2;

    /// <summary>
    /// Indicates whether the parameter is splitted.
    /// </summary>
    /// <param name="keySpan"></param>
    /// <returns></returns>
    internal static bool IsParameterSplitted(this ReadOnlySpan<char> keySpan) => keySpan.GetSplitIndicatorIndex() != -1;

    /// <summary>
    /// Gets the index of the first '*' sign after the key name, which indicates that the parameter is splitted. ("key*0" or "key*0*" or "key*42*")
    /// </summary>
    /// <param name="keySpan">A span that contains the parameters key without the equals sign.</param>
    /// <returns>The index of the split indicator if found, otherwise -1.</returns>
    internal static int GetSplitIndicatorIndex(this ReadOnlySpan<char> keySpan)
    {
        int idx = keySpan.Length - 2;

        // At least one letter has the key name to be long.
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

    /// <summary>
    /// Splits a long <see cref="MimeTypeParameter"/> into several parts and returns them as a collection of <see cref="StringBuilder"/>
    /// objects.
    /// </summary>
    /// <param name="parameter">The <see cref="MimeTypeParameter"/> to split.</param>
    /// <param name="worker">A <see cref="StringBuilder"/> that holds the serialized <paramref name="parameter"/>.</param>
    /// <param name="lineLength">The line length at which the parameter should be splitted.</param>
    /// <param name="enc">The <see cref="EncodingAction"/> the <paramref name="parameter"/> had been serialized to worker.</param>
    /// <returns>A collection of <see cref="StringBuilder"/> objects that represents the splitted <paramref name="parameter"/>.</returns>
    internal static IEnumerable<StringBuilder> SplitParameter(MimeTypeParameter parameter, StringBuilder worker, int lineLength, EncodingAction enc)
    {
        Debug.Assert(worker.Length > 0);
        const int COUNTER_INITIAL_LENGTH = 1;

        RemoveKeyFromWorker(worker, enc);
        int minimumLength = ComputeMinimumLineLength(parameter, enc);

        if (lineLength < minimumLength)
        {
            lineLength = minimumLength;
        }

        var tmp = new StringBuilder(lineLength);
        _ = tmp.Append(parameter.Key).Append('*');

        int counterIdx = tmp.Length;
        int counter = 0;
        _ = tmp.Append(counter);

        if (enc == EncodingAction.UrlEncode)
        {
            _ = tmp.Append('*');
        }
        _ = tmp.Append('=');

        int normalValueStart = tmp.Length - COUNTER_INITIAL_LENGTH;

        if (enc == EncodingAction.UrlEncode)
        {
            _ = tmp.Append(ParameterSerializer.UTF_8).Append('\'').Append(parameter.Language).Append('\'');
        }

        int valueStart = tmp.Length;
        int valLength;
        bool quoted = enc.HasFlag(EncodingAction.Quote);

        do
        {
            valLength = UpdateLineLength(worker.Length, lineLength, quoted, valueStart);
            MoveChunk(worker, quoted, tmp, valLength);

            yield return tmp;

            if (worker.Length == 0)
            {
                yield break;
            }

            tmp.ClearValue(counter, normalValueStart)
               .UpdateCounter(counterIdx, ref counter);

            valueStart = normalValueStart + counter.DigitsCount();

        } while (valLength > 5); // Security: if counter gets too large valLength could become negative
    }

    private static int UpdateLineLength(int workerLength, int lineLength, bool quoted, int valueStart)
    {
        int valLength = lineLength - valueStart;
        if (quoted)
        {
            valLength -= DOUBLE_QUOTES_COUNT;
        }

        // The rest of the last line:
        if (valLength > workerLength)
        {
            valLength = workerLength;
        }

        return valLength;
    }

    /// <summary>
    /// Removes anything from worker except the parameter value.
    /// </summary>
    /// <param name="worker"></param>
    /// <param name="quoted"></param>
    /// <param name="urlEncoded"></param>
    private static void RemoveKeyFromWorker(StringBuilder worker, EncodingAction enc)
    {
        int startOfValue = worker.IndexOf('=') + 1;

        if (enc == EncodingAction.UrlEncode)
        {
            startOfValue = worker.IndexOf('\'', startOfValue);
            startOfValue = worker.IndexOf('\'', startOfValue + 1) + 1;
        }
        else if (enc.HasFlag(EncodingAction.Quote))
        {
            startOfValue++; // ="
            _ = worker.Remove(worker.Length - 1, 1);
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
    private static int ComputeMinimumLineLength(MimeTypeParameter parameter, EncodingAction enc)
    {
        int minimumLength = parameter.Key.Length + 9; // *0= and 6 value chars per line at least

        if (enc == EncodingAction.UrlEncode)
        {
            minimumLength += 3 + parameter.CharSet.Length + parameter.Language.Length; // *''
        }
        else if (enc.HasFlag(EncodingAction.Quote))
        {
            minimumLength += 2; // ""
        }

        return minimumLength;
    }

    /// <summary>
    /// Moves a chunk of the parameters value from <paramref name="worker"/> to <paramref name="tmp"/>.
    /// </summary>
    /// <param name="worker"></param>
    /// <param name="quoted">Indicates whether the chunk shall be quoted in <paramref name="tmp"/>.</param>
    /// <param name="tmp"></param>
    /// <param name="chunkLength"></param>
    private static void MoveChunk(StringBuilder worker, bool quoted, StringBuilder tmp, int chunkLength)
    {
        if (quoted)
        {
            tmp.Append('\"');
        }

        _ = tmp.Append(worker, 0, chunkLength);
        _ = worker.Remove(0, chunkLength);

        if (quoted)
        {
            tmp.Append('\"');
        }
    }

    /// <summary>
    /// Clears the value from <paramref name="builder"/>, beginning at <paramref name="valueStart"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="counter"></param>
    /// <param name="valueStart"></param>
    /// <returns><paramref name="builder"/></returns>
    private static StringBuilder ClearValue(this StringBuilder builder, int counter, int valueStart)
    {
        valueStart += counter.DigitsCount();
        _ = builder.Remove(valueStart, builder.Length - valueStart);
        return builder;
    }

    /// <summary>
    /// Updates the <paramref name="counter"/> in <paramref name="builder"/> to be prepared
    /// for the next iteration.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="counterIdx"></param>
    /// <param name="counter">The current counter. After the method has returned, the current counter
    /// that is in <paramref name="builder"/>.</param>
    private static void UpdateCounter(this StringBuilder builder, int counterIdx, ref int counter)
    {
        builder.Remove(counterIdx, counter.DigitsCount());
        int i = ++counter;
        do
        {
            builder.Insert(counterIdx, (char)(i % 10 + '0'));
            i /= 10;
        }
        while (i != 0);
    }

}
