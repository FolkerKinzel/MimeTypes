using System;
using System.Text;
using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;

namespace FolkerKinzel.MimeTypes
{
    public readonly partial struct MimeTypeParameter
    {
        private static class MimeTypeParameterBuilder
        {
            internal static void BuildUrlEncoded(StringBuilder builder, in MimeTypeParameter parameter, bool isValueAscii)
            {
                ReadOnlySpan<char> valueSpan = parameter.Value;
                ReadOnlySpan<char> keySpan = parameter.Key;
                ReadOnlySpan<char> languageSpan = parameter.Language;

                try
                {
                    valueSpan = Uri.EscapeDataString(valueSpan.ToString()).AsSpan();
                }
                catch (FormatException)
                {
                    return;
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
                _ = parameter.IsValueCaseSensitive
                    ? builder.Append(valueSpan)
                    : builder.Append(valueSpan).ToLowerInvariant(valueStart);
            }

            internal static void BuildMasked(StringBuilder builder, in MimeTypeParameter parameter, bool containsMaskChars)
            {
                ReadOnlySpan<char> valueSpan = parameter.Value;
                ReadOnlySpan<char> keySpan = parameter.Key;

                if (containsMaskChars)
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

                _ = builder.Append('\"');
            }

            internal static void BuildUnmasked(StringBuilder builder, in MimeTypeParameter parameter)
            {
                ReadOnlySpan<char> valueSpan = parameter.Value;
                ReadOnlySpan<char> keySpan = parameter.Key;

                //                                                       =
                int neededCapacity = valueSpan.Length + keySpan.Length + 1;
                _ = builder.EnsureCapacity(builder.Length + neededCapacity);

                int keyStart = builder.Length;
                _ = builder.Append(keySpan).ToLowerInvariant(keyStart).Append('=');

                int valueStart = builder.Length;
                _ = parameter.IsValueCaseSensitive
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
}