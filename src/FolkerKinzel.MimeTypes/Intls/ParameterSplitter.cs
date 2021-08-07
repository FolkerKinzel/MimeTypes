using System;
using System.Collections.Generic;
using System.Text;
using FolkerKinzel.Strings;
using System.Linq;
using FolkerKinzel.Strings.Polyfills;

namespace FolkerKinzel.MimeTypes.Intls
{
    internal static class ParameterSplitter
    {
        internal static IEnumerable<StringBuilder> SplitParameter(MimeTypeParameter parameter, StringBuilder worker, int lineLength)
        {
            bool quoted = worker[worker.Length - 1] == '"';
            bool urlEncoded = !quoted && worker.Contains('%');

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
            int valueStart = tmp.Length;
            if (urlEncoded)
            {
                _ = tmp.Append(parameter.Charset).Append('\'').Append(parameter.Language).Append('\'');
            }

            int valLength = lineLength - valueStart;
            if (quoted)
            {
                valLength -= 2;
            }

            do
            {
                if (worker.Length == 0)
                {
                    yield break;
                }

                _ = tmp.Remove(valueStart, tmp.Length - valueStart);

                tmp[counterIdx] = (char)(counter + '0');
                counter++;

                if (valLength > worker.Length)
                {
                    valLength = worker.Length;
                }


                if (urlEncoded)
                {
                    if (worker.Length == valLength)
                    {
                        _ = tmp.Append(worker);
                        //_ = worker.Clear();

                        yield return tmp;
                        yield break;
                    }

                    int chunkLength = ComputeUrlEncodedChunkLength(worker, valLength);

                    _ = tmp.Append(worker, 0, chunkLength);
                    _ = worker.Remove(0, chunkLength);
                }
                else if (quoted)
                {
                    if (worker.Length == valLength)
                    {
                        _ = tmp.Append('\"').Append(worker).Append('\"');
                        //_ = worker.Clear();

                        yield return tmp;
                        yield break;
                    }

                    int chunkLength = ComputeQuotedChunkLength(worker, valLength);

                    _ = tmp.Append('\"').Append(worker, 0, chunkLength).Append('\"');
                    _ = worker.Remove(0, chunkLength);
                }
                else
                {
                    _ = tmp.Append(worker, 0, valLength);
                    _ = worker.Remove(0, valLength);
                }

            } while (counter < 10);
        }

        private static int ComputeUrlEncodedChunkLength(StringBuilder worker, int chunkLength)
        {
            for (int i = chunkLength - 1; i >= 0; i--)
            {
                char current = worker[i];

                if (Uri.IsHexDigit(current))
                {
                    if (i < chunkLength - 3)
                    {
                        break;
                    }
                }
                else
                {
                    chunkLength = current == '%'
                                    ? i < chunkLength - 3
                                            ? chunkLength
                                            : i
                                     : i + 1;
                    break;
                }
            }

            return chunkLength;
        }

        private static int ComputeQuotedChunkLength(StringBuilder worker, int chunkLength)
        {
            int i;
            for (i = chunkLength - 1; i >= 0; i--)
            {
                char current = worker[chunkLength - 1];

                if (current != '\\')
                {
                    chunkLength = i + 1;
                    break;
                }
            }

            if (i < 0)
            {
                // only \\\\\\\\\\\\\\\\\\\\\\\\

                chunkLength -= chunkLength % 2;
            }

            return chunkLength;
        }


        private static void PrepareWorker(StringBuilder worker, bool quoted, bool urlEncoded)
        {
            int keyLength = worker.IndexOf('=') + 1;
            if (quoted)
            {
                keyLength++; // ="
                _ = worker.Remove(worker.Length - 1, 1);
            }
            else if (urlEncoded)
            {
                keyLength = worker.IndexOf('\'', keyLength);
                keyLength = worker.IndexOf('\'', keyLength + 1);
            }
            _ = worker.Remove(0, keyLength);
        }

        private static int ComputeMinimumLength(MimeTypeParameter parameter, bool quoted, bool urlEncoded)
        {
            int minimumLength = parameter.Key.Length + 9; // *0= and 6 value chars per line at least

            if (quoted)
            {
                minimumLength += 2; // ""
            }
            else if (urlEncoded)
            {
                minimumLength += 3 + parameter.Charset.Length + parameter.Language.Length; // *''
            }

            return minimumLength;
        }
    }
}