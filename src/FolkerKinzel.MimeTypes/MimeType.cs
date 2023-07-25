namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Static class that works on <see cref="string"/>s and offers methods which are able to retrieve an appropriate file type extension 
/// for a given Internet Media Type ("MIME type") or
/// an appropriate Internet Media Type for a given file type extension.
/// </summary>
/// <example>
/// <para>
/// Get an Internet Media Type from a file type extension and vice versa:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
/// </example>
public static class MimeType
{
    /// <summary>
    /// Gets an appropriate file type extension for <paramref name="mimeType"/>.
    /// </summary>
    /// <param name="mimeType">A <see cref="string"/> that represents an Internet Media Type ("MIME type") or <c>null</c>.</param>
    /// <param name="includePeriod"><c>true</c> specifies, that the period "." (U+002E) is included in the retrieved file type 
    /// extension, <c>false</c>, that it's not.</param>
    /// <returns>An appropriate file type extension for <paramref name="mimeType"/>.</returns>
    /// <remarks>
    /// <para>
    /// If no other file type extension could be found, <see cref="MimeCache.DefaultFileTypeExtension"/>
    /// is returned. <paramref name="includePeriod"/> specifies whether the period is included.
    /// </para>
    /// <para>
    /// Internally a small memory cache is used to retrieve often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or you can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if your application does not need it anymore.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Get an Internet Media Type from a file type extension and vice versa:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    public static string GetFileTypeExtension(string? mimeType, bool includePeriod = true)
    {
        if (mimeType != null)
        {
            int parameterStartIdx = mimeType.IndexOf(';');

            if (parameterStartIdx >= 0)
            {
                mimeType = mimeType.Substring(0, parameterStartIdx);
            }

            mimeType = string.IsNullOrWhiteSpace(mimeType)
                ? null
                : mimeType.ReplaceWhiteSpaceWith(ReadOnlySpan<char>.Empty).ToLowerInvariant();
        }
        return MimeCache.GetFileTypeExtension(mimeType, includePeriod);
    }


    /// <summary>
    /// Gets an appropriate file type extension for <paramref name="mimeType"/>.
    /// </summary>
    /// <param name="mimeType">A read-only character span that represents an Internet Media Type ("MIME type").</param>
    /// <param name="includePeriod"><c>true</c> specifies, that the period "." (U+002E) is included in the retrieved file type 
    /// extension, <c>false</c>, that it's not.</param>
    /// <returns>An appropriate file type extension for <paramref name="mimeType"/>.</returns>
    /// <remarks>
    /// <para>
    /// If no other file type extension could be found, <see cref="MimeCache.DefaultFileTypeExtension"/>
    /// is returned. <paramref name="includePeriod"/> specifies whether the period is included.
    /// </para>
    /// <para>
    /// Internally a small memory cache is used to retrieve often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or you can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if your application does not need it anymore.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Get an Internet Media Type from a file type extension and vice versa:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    public static string GetFileTypeExtension(ReadOnlySpan<char> mimeType, bool includePeriod = true)
    {
        int parameterStartIdx = mimeType.IndexOf(';');

        if (parameterStartIdx >= 0)
        {
            mimeType = mimeType.Slice(0, parameterStartIdx);
        }

        string? mimeTypeString = mimeType.IsWhiteSpace()
            ? null
            : mimeType.ToString().ReplaceWhiteSpaceWith(ReadOnlySpan<char>.Empty).ToLowerInvariant();

        return MimeCache.GetFileTypeExtension(mimeTypeString, includePeriod);
    }

    /// <summary>
    /// Gets an appropriate Internet Media Type ("MIME type") for a given file type extension.
    /// </summary>
    /// <param name="fileTypeExtension">The file type extension or <c>null</c>.</param>
    /// <returns>An appropriate Internet Media Type ("MIME type") for <paramref name="fileTypeExtension"/>.</returns>
    /// <remarks>
    /// <para>
    /// If no other Internet Media Type could be found, <see cref="MimeCache.DefaultMimeType"/>
    /// is returned.
    /// </para>
    /// <para>
    /// Internally a small memory cache is used to retrieve often used Internet Media Types faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or you can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if your application does not need it anymore.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Get an Internet Media Type from a file type extension and vice versa:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromFileTypeExtension(string? fileTypeExtension) => MimeCache.GetMimeType(fileTypeExtension);


    /// <summary>
    /// Gets an appropriate Internet Media Type ("MIME type") for a given file type extension.
    /// </summary>
    /// <param name="fileTypeExtension">The file type extension or <c>null</c>.</param>
    /// <returns>An appropriate Internet Media Type ("MIME type") for <paramref name="fileTypeExtension"/>.</returns>
    /// <remarks>
    /// <para>
    /// If no other Internet Media Type could be found, <see cref="MimeCache.DefaultMimeType"/>
    /// is returned.
    /// </para>
    /// <para>
    /// Internally a small memory cache is used to retrieve often used Internet Media Types faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or you can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if your application does not need it anymore.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Get an Internet Media Type from a file type extension and vice versa:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromFileTypeExtension(ReadOnlySpan<char> fileTypeExtension) => MimeCache.GetMimeType(fileTypeExtension);
}
