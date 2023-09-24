using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;
using System.Runtime.ConstrainedExecution;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

internal static class ParameterSplitter
{
    private const int DOUBLE_QUOTES_COUNT = 2;
    private const int COUNTER_INITIAL_LENGTH = 1;

    /// <summary>
    /// *0= and 6 value chars per line at least
    /// </summary>
    internal const int MINIMUM_VARIABLE_LINE_LENGTH = 9;


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

        // One letter is the minimum length of a key.
        while (idx > 0)
        {
            char c = keySpan[idx];
            if (c == '*')
            {
                return idx;
            }
            else if (!c.IsAsciiDigit())
            {
                return -1;
            }

            idx--;
        }

        return -1;
    }

    /// <summary>
    /// Splits a long Internet Media Type parameter into several parts and returns them as a collection of <see cref="StringBuilder"/>
    /// objects.
    /// </summary>
    /// <param name="worker">A <see cref="StringBuilder"/> that holds the serialized parameter.</param>
    /// <param name="lineLength">The line length at which the parameter should be splitted.</param>
    /// <param name="enc">The <see cref="EncodingAction"/> the parameter had been serialized to worker.</param>
    /// 
    /// 
    /// <returns>A collection of <see cref="StringBuilder"/> objects that represents the splitted parameter.</returns>
    internal static IEnumerable<StringBuilder> SplitParameter(StringBuilder worker,
                                                              int lineLength,
                                                              EncodingAction enc)
    {
        Debug.Assert(worker.Length > lineLength);

        var tmp = new StringBuilder(lineLength);
        (int counterIdx, int normalValueStart) = PrepareBuilders(worker, tmp, enc);

        int valueStart = tmp.Length;
        bool quoted = enc.HasFlag(EncodingAction.Quote);
        int counter = 0;
        int valLength;

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

    private static (int counterIdx, int normalValueStart) PrepareBuilders(StringBuilder worker,
                                                                          StringBuilder tmp,
                                                                          EncodingAction enc)
    {
        var tpl = PrepareTmp(worker, tmp, enc);
        RemoveKeyFromWorker(worker, enc);
        return tpl;
    }

    private static (int counterIdx, int normalValueStart) PrepareTmp(StringBuilder worker,
                                                                     StringBuilder tmp,
                                                                     EncodingAction enc)
    {
        CopyKeyIntoTmp(worker, tmp);
        _ = tmp.Append('*');

        int counterIdx = tmp.Length;
        _ = tmp.Append('0');

        if (enc == EncodingAction.UrlEncode)
        {
            _ = tmp.Append('*');
        }
        _ = tmp.Append('=');

        int normalValueStart = tmp.Length - COUNTER_INITIAL_LENGTH;

        if (enc == EncodingAction.UrlEncode)
        {
            _ = tmp.Append(ParameterSerializer.UTF_8);
                CopyLanguageIntoTmp(worker, tmp);
        }

        return (counterIdx, normalValueStart);

        //////////////////////////////////////////////

        static void CopyKeyIntoTmp(StringBuilder source, StringBuilder tmp)
        {
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (c.IsTokenChar(true))
                {
                    tmp.Append(c); 
                }
                else
                {
                    return;
                }
            }
        }

        static void CopyLanguageIntoTmp(StringBuilder source, StringBuilder tmp)
        {
            const char languageIndicator = '\'';

            _ = tmp.Append(languageIndicator);
                   
            for (int i = source.IndexOf(languageIndicator) + 1; i < source.Length; i++)
            {
                char c = source[i];
                tmp.Append(c);

                if (c == languageIndicator)
                {
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Removes anything from worker except the parameter value.
    /// </summary>
    /// <param name="worker"></param>
    /// <param name="enc">The <see cref="EncodingAction"/> that had been used to
    /// serialize the content in <paramref name="worker"/>.</param>
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
