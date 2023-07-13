using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using System.Data.Common;
using System.Text;

namespace FolkerKinzel.MimeTypes.Intls;

internal static class ParameterBuilder
{
    private const string UTF_8 = "utf-8";
    internal static void Build(StringBuilder builder, in MimeTypeParameterModel model)
    {
        Debug.Assert(builder is not null);

        ReadOnlySpan<char> valueSpan = model.Value.AsSpan();
        ReadOnlySpan<char> languageSpan = model.Language.AsSpan();
        bool isValueAscii = valueSpan.IsAscii();
        bool hasValue = !valueSpan.IsEmpty;

        bool starred = !languageSpan.IsEmpty || !isValueAscii;


        if (starred && hasValue)
        {
            try
            {
                valueSpan = Uri.EscapeDataString(model.Value!).AsSpan();
            }
            catch
            {
                return;
            }
        }

        TSpecialKinds tSpecialKind = starred ? TSpecialKinds.None
                                             : hasValue
                                                ? valueSpan.AnalyzeTSpecials()
                                                : TSpecialKinds.TSpecial;
        _ = builder.Append(model.Key);

        _ = tSpecialKind switch
        {
            TSpecialKinds.MaskChar => builder.Append('=').AppendValueQuotedAndMasked(valueSpan, true),
            TSpecialKinds.TSpecial => builder.Append('=').AppendValueQuoted(valueSpan, true),

            // This adds '=':
            _ => AppendValueUrlEncoded(builder, isValueAscii, valueSpan, languageSpan, starred),
        };
    }


    internal static StringBuilder Build(StringBuilder builder, in MimeTypeParameter parameter, bool urlEncodedValue)
    {
        ReadOnlySpan<char> valueSpan = parameter.Value;

        bool isValueAscii = valueSpan.IsAscii();
        bool urlEncoded = urlEncodedValue || !parameter.Language.IsEmpty || !isValueAscii;

        if (urlEncoded)
        {
            return builder.BuildUrlEncoded(in parameter, isValueAscii);
        }

        // See https://mimesniff.spec.whatwg.org/#serializing-a-mime-type :
        // Empty values should be Double-Quoted.
        TSpecialKinds tSpecialKind = valueSpan.IsEmpty ? TSpecialKinds.TSpecial : valueSpan.AnalyzeTSpecials();

        return tSpecialKind == TSpecialKinds.None
                ? builder.BuildUnQuoted(in parameter)
                : builder.BuildQuoted(in parameter, tSpecialKind);
    }


    private static StringBuilder BuildUrlEncoded(this StringBuilder builder, in MimeTypeParameter parameter, bool isValueAscii)
    {
        ReadOnlySpan<char> keySpan = parameter.Key;
        ReadOnlySpan<char> valueSpan = parameter.Value;
        ReadOnlySpan<char> languageSpan = parameter.Language;

        try
        {
            valueSpan = Uri.EscapeDataString(valueSpan.ToString())
                           .AsSpan();
        }
        catch
        {
            return builder;
        }

        bool starred = !isValueAscii || !languageSpan.IsEmpty;
        _ = builder.EnsureCapacity(builder.Length + GetNeededCapacity(keySpan.Length, valueSpan.Length, languageSpan.Length, isValueAscii, starred));
        return builder.BuildKey(keySpan) // adds '='
                      .Remove(builder.Length - 1, 1) // removes '='
                      .AppendValueUrlEncoded(isValueAscii, valueSpan, languageSpan, starred); // adds '='

        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        static int GetNeededCapacity(int keySpanLength, int valueSpanLength, int languageSpanLength, bool isValueAscii, bool starred)
        {

            int charsetLength = isValueAscii ? 0 : UTF_8.Length;

            //                                                     =
            int neededCapacity = valueSpanLength + keySpanLength + 1;
            if (starred)
            {
                //                *                  ' '
                neededCapacity += 1 + charsetLength + 2 + languageSpanLength;
            }
            return neededCapacity;
        }
    }

    private static StringBuilder BuildQuoted(this StringBuilder builder, in MimeTypeParameter parameter, TSpecialKinds tSpecialKind)
    {
        ReadOnlySpan<char> valueSpan = parameter.Value;
        ReadOnlySpan<char> keySpan = parameter.Key;
        bool isValueCaseSensitive = parameter.IsValueCaseSensitive;

        bool masked = tSpecialKind == TSpecialKinds.MaskChar;

        int neededCapacity = 2 + (masked ? valueSpan.Length + 2 : valueSpan.Length) + keySpan.Length + 1;
        _ = builder.EnsureCapacity(builder.Length + neededCapacity);

        _ = builder.BuildKey(keySpan); // adds '='
        return masked ? builder.AppendValueQuotedAndMasked(valueSpan, isValueCaseSensitive)
                      : builder.AppendValueQuoted(valueSpan, isValueCaseSensitive);
    }


    private static StringBuilder BuildUnQuoted(this StringBuilder builder, in MimeTypeParameter parameter)
    {
        ReadOnlySpan<char> valueSpan = parameter.Value;
        ReadOnlySpan<char> keySpan = parameter.Key;

        //                                                       =
        int neededCapacity = valueSpan.Length + keySpan.Length + 1;
        _ = builder.EnsureCapacity(builder.Length + neededCapacity);

        return builder.BuildKey(keySpan).AppendValueUnQuoted(valueSpan, parameter.IsValueCaseSensitive);
    }

    private static StringBuilder BuildKey(this StringBuilder builder, ReadOnlySpan<char> keySpan)
    {
        int keyStart = builder.Length;
        return builder.Append(keySpan).ToLowerInvariant(keyStart).Append('=');
    }

    private static StringBuilder AppendValueUrlEncoded(this StringBuilder builder,
                                                     bool isValueAscii,
                                                     ReadOnlySpan<char> valueSpan,
                                                     ReadOnlySpan<char> languageSpan,
                                                     bool starred)
    {
        return starred
            ? builder.Append('*').Append('=').Append(isValueAscii ? "" : UTF_8).Append('\'').Append(languageSpan).Append('\'').AppendValueUnQuoted(valueSpan, true)
            : builder.Append('=').AppendValueUnQuoted(valueSpan, true);
    }

    private static StringBuilder AppendValueQuoted(this StringBuilder builder, ReadOnlySpan<char> valueSpan, bool isValueCaseSensitive) =>
        builder.Append('\"').AppendValueUnQuoted(valueSpan, isValueCaseSensitive).Append('\"');


    private static StringBuilder AppendValueQuotedAndMasked(this StringBuilder builder,
                                                     ReadOnlySpan<char> value,
                                                     bool isValueCaseSensitive)
    {
        _ = builder.Append('\"');

        int valueStart = builder.Length;
        _ = isValueCaseSensitive
                ? builder.AppendMasked(value)
                : builder.AppendMasked(value).ToLowerInvariant(valueStart);

        return builder.Append('\"');
    }


    private static StringBuilder AppendValueUnQuoted(this StringBuilder builder, ReadOnlySpan<char> valueSpan, bool isValueCaseSensitive)
    {
        int valueStart = builder.Length;
        return isValueCaseSensitive
                ? builder.Append(valueSpan)
                : builder.Append(valueSpan).ToLowerInvariant(valueStart);
    }


    private static StringBuilder AppendMasked(this StringBuilder sb, ReadOnlySpan<char> value)
    {
        sb.EnsureCapacity(sb.Length + value.Length * 2);

        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (c is '\"' or '\\')
            {
                _ = sb.Append('\\');
            }
            sb.Append(c);
        }

        return sb;
    }
}
