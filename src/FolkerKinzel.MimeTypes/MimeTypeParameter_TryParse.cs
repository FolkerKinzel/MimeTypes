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

        ParameterIndexes idx; 

        if (firstRun)
        {
            var sanitizer = new ParameterSanitizer();
            
            if (!sanitizer.RepairParameterString(ref parameterString, out int keyLength))
            {
                return false;
            }

            idx = new ParameterIndexes(parameterString.Span)
            {
                KeyLength = keyLength
            };
        }
        else
        {
            idx = new ParameterIndexes(parameterString.Span);
            idx.KeyLength = idx.Span.IndexOf(SEPARATOR);
            Debug.Assert(!idx.Span.Slice(0, idx.KeyLength).ContainsWhiteSpace());

            if (idx.KeyLength < 1)
            {
                return false;
            }
        }
        

        // Don't change the order of this statement: TryDecodeValue changes idx!
        if(!ParameterValueDecoder.TryDecodeValue(firstRun, ref idx, ref parameterString) || !idx.Verify())
        {
            return false;
        }

        int parameterIdx = InitParameterIdx(in idx);

        parameter = new MimeTypeParameter(in parameterString, parameterIdx);
        return true;

        /////////////////////////////////////////////////////////////////////////////////////

        static int InitParameterIdx(in ParameterIndexes idx)
        {
            int parameterIdx = idx.KeyLength;
            parameterIdx |= idx.KeyValueOffset << KEY_VALUE_OFFSET_SHIFT;

            if (idx.LanguageStart != 0)
            {
                parameterIdx |= 1 << CHARSET_LANGUAGE_INDICATOR_SHIFT;
                parameterIdx |= idx.CharsetLength << CHARSET_LENGTH_SHIFT;
                parameterIdx |= idx.LanguageLength << LANGUAGE_LENGTH_SHIFT;
            }

            return parameterIdx;
        }
    }

 

}
