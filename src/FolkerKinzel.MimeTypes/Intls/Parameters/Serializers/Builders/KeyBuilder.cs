using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class KeyBuilder
{
    internal const int KEY_VALUE_SEPARATOR_LENGTH = MimeTypeParameter.EQUALS_SIGN_LENGTH;

    internal static StringBuilder BuildKey(this StringBuilder builder, ReadOnlySpan<char> keySpan)
    {
        int keyStart = builder.Length;
        return builder.Append(keySpan).ToLowerInvariant(keyStart).Append('=');
    }

}
