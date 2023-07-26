using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers.Builders;

internal static class KeyBuilder
{
    internal const int KEY_VALUE_SEPARATOR_LENGTH = MimeTypeParameterInfo.EQUALS_SIGN_LENGTH;

    internal static StringBuilder BuildKey(this StringBuilder builder, string key)
    {
        int keyStart = builder.Length;
        return builder.Append(key).ToLowerInvariant(keyStart).Append('=');
    }

}
