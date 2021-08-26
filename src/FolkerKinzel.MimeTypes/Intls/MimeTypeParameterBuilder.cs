using System;
using System.Text;
using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;

namespace FolkerKinzel.MimeTypes.Intls
{
    internal static class MimeTypeParameterBuilder
    {
        internal static void Build(StringBuilder builder, in ParameterModel model)
        {
            string? value = model.Value;
            bool isValueAscii = value is null || value.IsAscii();
            bool hasLanguage = model.Language is not null;

            bool urlEncoded = hasLanguage || !isValueAscii;


            if (urlEncoded && value is not null)
            {
                try
                {
                    value = Uri.EscapeDataString(value);
                }
                catch
                {
                    return;
                }
            }

            _ = builder.Append(model.Key);

            if (hasLanguage || !isValueAscii)
            {
                _ = builder.Append('*').Append('=').Append(isValueAscii ? "" : "utf-8").Append('\'').Append(model.Language).Append('\'').Append(value);
                return;
            }
            else if (urlEncoded)
            {
                _ = builder.Append('=').Append(value);
                return;
            }

            TSpecialKinds tSpecialKind = value is null ? TSpecialKinds.TSpecial : value.AsSpan().AnalyzeTSpecials();

            if (tSpecialKind == TSpecialKinds.MaskChar)
            {
                var sb = new StringBuilder(value!.Length * 2);
                _ = sb.Append(value);
                value = Mask(sb).ToString();

                _ = builder.Append('=').Append('\"').Append(value).Append('\"');
                return;
            }
            else if (tSpecialKind == TSpecialKinds.TSpecial)
            {
                _ = builder.Append('=').Append('\"').Append(value).Append('\"');
                return;
            }

            _ = builder.Append('=').Append(value);
        }

        internal static StringBuilder Build(StringBuilder builder, in MimeTypeParameter parameter, bool urlEncodedValue)
        {
            ReadOnlySpan<char> valueSpan = parameter.Value;

            bool isValueAscii = valueSpan.IsAscii();
            bool urlEncoded = urlEncodedValue || !parameter.Language.IsEmpty || !isValueAscii;

            if (urlEncoded)
            {
                return MimeTypeParameterBuilder.BuildUrlEncoded(builder, in parameter, isValueAscii);
            }

            // See https://mimesniff.spec.whatwg.org/#serializing-a-mime-type :
            // Empty values should be Double-Quoted.
            TSpecialKinds tSpecialKind = valueSpan.IsEmpty ? TSpecialKinds.TSpecial : valueSpan.AnalyzeTSpecials();

            return tSpecialKind == TSpecialKinds.None
                    ? MimeTypeParameterBuilder.BuildUnQuoted(builder, in parameter)
                    : MimeTypeParameterBuilder.BuildQuoted(builder, in parameter, tSpecialKind);
        }


        private static StringBuilder BuildUrlEncoded(StringBuilder builder, in MimeTypeParameter parameter, bool isValueAscii)
        {
            ReadOnlySpan<char> valueSpan = parameter.Value;
            ReadOnlySpan<char> keySpan = parameter.Key;
            ReadOnlySpan<char> languageSpan = parameter.Language;

            try
            {
                valueSpan = Uri.EscapeDataString(valueSpan.ToString())
                               .AsSpan();
            }
            catch (FormatException)
            {
                return builder;
            }

            bool starred = !isValueAscii || !languageSpan.IsEmpty;

            const string utf8 = "utf-8";
            int charsetLength = isValueAscii ? 0 : utf8.Length;

            //                                                       =
            int neededCapacity = valueSpan.Length + keySpan.Length + 1;

            if (starred)
            {
                //                *                  ' '
                neededCapacity += 1 + charsetLength + 2 + languageSpan.Length;
            }

            _ = builder.EnsureCapacity(builder.Length + neededCapacity);

            int keyStart = builder.Length;
            _ = builder.Append(keySpan).ToLowerInvariant(keyStart);

            _ = starred
                ? builder.Append('*').Append('=').Append(isValueAscii ? "" : utf8).Append('\'').Append(languageSpan).Append('\'')
                : builder.Append('=');

            int valueStart = builder.Length;
            return parameter.IsValueCaseSensitive
                    ? builder.Append(valueSpan)
                    : builder.Append(valueSpan).ToLowerInvariant(valueStart);
        }

        private static StringBuilder BuildQuoted(StringBuilder builder, in MimeTypeParameter parameter, TSpecialKinds tSpecialKind)
        {
            ReadOnlySpan<char> valueSpan = parameter.Value;
            ReadOnlySpan<char> keySpan = parameter.Key;

            if (tSpecialKind == TSpecialKinds.MaskChar)
            {
                var sb = new StringBuilder(valueSpan.Length * 2);
                _ = sb.Append(valueSpan);
                valueSpan = Mask(sb).ToString().AsSpan();
            }

            int neededCapacity = 2 + valueSpan.Length + keySpan.Length + 1;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);

            int keyStart = builder.Length;
            _ = builder.Append(keySpan).ToLowerInvariant(keyStart).Append('=');

            _ = builder.Append('\"');

            int valueStart = builder.Length;
            _ = parameter.IsValueCaseSensitive
                ? builder.Append(valueSpan)
                        : builder.Append(valueSpan).ToLowerInvariant(valueStart);

            return builder.Append('\"');
        }

        private static StringBuilder BuildUnQuoted(StringBuilder builder, in MimeTypeParameter parameter)
        {
            ReadOnlySpan<char> valueSpan = parameter.Value;
            ReadOnlySpan<char> keySpan = parameter.Key;

            //                                                       =
            int neededCapacity = valueSpan.Length + keySpan.Length + 1;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);

            int keyStart = builder.Length;
            _ = builder.Append(keySpan).ToLowerInvariant(keyStart).Append('=');

            int valueStart = builder.Length;
            return parameter.IsValueCaseSensitive
                    ? builder.Append(valueSpan)
                    : builder.Append(valueSpan).ToLowerInvariant(valueStart);
        }

        private static StringBuilder Mask(StringBuilder sb)
        {
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                if (sb[i] is '"' or '\\')
                {
                    _ = sb.Insert(i, '\\');
                }
            }

            return sb;
        }
    }
}