namespace FolkerKinzel.MimeTypes.Intls;

#pragma warning disable IDE1006 // Benennungsstile

internal static class _Int
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Parse(ReadOnlySpan<char> s) =>
#if NETSTANDARD2_0 || NET461
    int.Parse(s.ToString());
#else
    int.Parse(s);
#endif


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TryParse(ReadOnlySpan<char> s, out int i) =>
#if NETSTANDARD2_0 || NET461
    int.TryParse(s.ToString(), out i);
#else
    int.TryParse(s, out i);
#endif
}

#pragma warning restore IDE1006 // Benennungsstile
