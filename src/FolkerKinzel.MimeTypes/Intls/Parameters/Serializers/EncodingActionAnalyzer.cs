namespace FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

internal static class EncodingActionAnalyzer
{
    internal static EncodingAction EncodingAction(this ReadOnlySpan<char> span)
    {
        // See https://mimesniff.spec.whatwg.org/#serializing-a-mime-type :
        // Empty values should be Double-Quoted.
        if (span.IsEmpty)
        {
            return Serializers.EncodingAction.Quote;
        }

        EncodingAction action = Serializers.EncodingAction.None;

        for (int i = 0; i < span.Length; i++)
        {
            char c = span[i];

            if (!c.IsAscii())
            {
                return Serializers.EncodingAction.UrlEncode;
            }

            if (c.IsMaskChar())
            {
                action = Serializers.EncodingAction.Mask;
            }

            if (!c.IsTokenChar(false) && action == Serializers.EncodingAction.None)
            {
                action = Serializers.EncodingAction.Quote;
            }
        }
        return action;
    }

    /// <summary>
    /// Indicates whether <paramref name="c"/> has to be masked with a backslash.
    /// </summary>
    /// <param name="c">The <see cref="char"/> to exmine.</param>
    /// <returns><c>true</c> if <paramref name="c"/> has to be masked because it is 
    /// a double quote or a backslash.</returns>
    /// <remarks>
    /// <para>
    /// In the RFCs is nothing said about what should happen if a '\"' appears inside
    /// of a parameter value.
    /// </para>
    /// <para>The suggestions came from
    /// https://mimesniff.spec.whatwg.org/#parsing-a-mime-type § 4.5.:
    /// </para>
    /// <list type="bullet">
    /// <item>4.1. Precede each occurrence of U+0022 (") or U+005C (\) in value with U+005C (\).</item>
    /// <item>4.2. Prepend U+0022 (") to value.</item>
    /// <item>4.3. Append U+0022 (") to value.</item>
    /// </list>
    /// </remarks>
    private static bool IsMaskChar(this char c) => c is '\\' or '\"';
}
