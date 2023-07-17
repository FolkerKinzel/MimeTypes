namespace FolkerKinzel.MimeTypes.Intls;

#pragma warning disable IDE1006 // Benennungsstile

internal static class _Int
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Parse(this ReadOnlySpan<char> s) =>
#if NETSTANDARD2_0 || NET461
    int.Parse(s.ToString());
#else
    int.Parse(s);
#endif
}

#pragma warning restore IDE1006 // Benennungsstile
