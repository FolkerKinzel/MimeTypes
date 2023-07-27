namespace FolkerKinzel.MimeTypes;

/// <summary>
/// Static class that works on <see cref="string"/>s and offers methods that perform conversions between Internet Media Type strings 
/// ("MIME types") and file names.
/// </summary>
/// <example>
/// <para>
/// Convert a file name into an Internet Media Type and get a file type extension from an Internet Media Type:
/// </para>
/// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
/// </example>
public static class MimeString
{
    /// <summary>
    /// Converts an Internet Media Type to an appropriate file type extension.
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
    /// This method doesn't perform any validation on <paramref name="mimeType"/>. If a strong validation of the input is needed,
    /// the instance methods <see cref="MimeTypeInfo.GetFileTypeExtension(bool)">MimeTypeInfo.GetFileTypeExtension(bool)</see> and
    /// <see cref="MimeType.GetFileTypeExtension(bool)">MimeType.GetFileTypeExtension(bool)</see> are better suited.
    /// </para>
    /// <para>
    /// Internally a small memory cache is used to retrieve often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or you can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if your application does not need it anymore.
    /// </para>
    /// </remarks>
    /// 
    /// <seealso cref="MimeTypeInfo.GetFileTypeExtension(bool)"/>
    /// <seealso cref="MimeType.GetFileTypeExtension(bool)"/>
    /// 
    /// <example>
    /// <para>
    /// Convert a file name into an Internet Media Type and get a file type extension from an Internet Media Type:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    public static string ToFileTypeExtension(string? mimeType, bool includePeriod = true)
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
    /// Converts an Internet Media Type to an appropriate file type extension.
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
    /// This method doesn't perform any validation on <paramref name="mimeType"/>. If a strong validation of the input is needed,
    /// the instance methods <see cref="MimeTypeInfo.GetFileTypeExtension(bool)">MimeTypeInfo.GetFileTypeExtension(bool)</see> and
    /// <see cref="MimeType.GetFileTypeExtension(bool)">MimeType.GetFileTypeExtension(bool)</see> are better suited.
    /// </para>
    /// <para>
    /// Internally a small memory cache is used to retrieve often used file type extensions faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or you can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if your application does not need it anymore.
    /// </para>
    /// </remarks>
    ///
    /// <seealso cref="MimeTypeInfo.GetFileTypeExtension(bool)"/>
    /// <seealso cref="MimeType.GetFileTypeExtension(bool)"/>
    /// 
    /// <example>
    /// <para>
    /// Convert a file name into an Internet Media Type and get a file type extension from an Internet Media Type:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    public static string ToFileTypeExtension(ReadOnlySpan<char> mimeType, bool includePeriod = true)
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
    /// Converts a file name into an Internet Media Type ("MIME type").
    /// </summary>
    /// <param name="fileName">A file path, file name, file type extension (no matter whether with or without the period "."), or <c>null</c>.</param>
    /// <returns>An appropriate Internet Media Type ("MIME type") for <paramref name="fileName"/>.</returns>
    /// <remarks>
    /// <para>
    /// If no other Internet Media Type could be found, <see cref="MimeType.Default"/> is returned.
    /// </para>
    /// <para>
    /// Internally a small memory cache is used to retrieve often used Internet Media Types faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or you can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if your application does not need it anymore.
    /// </para>
    /// </remarks>
    /// <seealso cref="MimeType.FromFileName(string?)"/>
    /// <seealso cref="MimeType.FromFileName(ReadOnlySpan{char})"/>
    /// 
    /// <example>
    /// <para>
    /// Convert a file name into an Internet Media Type and get a file type extension from an Internet Media Type:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromFileName(string? fileName)
    {
#if NET461 || NETSTANDARD2_0
        if (fileName != null)
        {
            try
            {
                string ext = Path.GetExtension(fileName);
                fileName = ext.Length > 0 ? ext : Path.GetFileName(fileName);
            }
            catch { fileName = null; }
        }
        
        return MimeCache.GetMimeType(fileName);
#else
        return FromFileName(fileName.AsSpan());
#endif
    }


    /// <summary>
    /// Converts a file name into an Internet Media Type ("MIME type").
    /// </summary>
    /// <param name="fileName">A file path, file name, file type extension (no matter whether with or without the period "."), or <c>null</c>.</param>
    /// <returns>An appropriate Internet Media Type ("MIME type") for <paramref name="fileName"/>.</returns>
    /// <remarks>
    /// <para>
    /// If no other Internet Media Type could be found, <see cref="MimeType.Default"/>
    /// is returned.
    /// </para>
    /// <para>
    /// Internally a small memory cache is used to retrieve often used Internet Media Types faster. You
    /// can enlarge the size of this cache with <see cref="MimeCache.EnlargeCapacity(int)">MimeCache.EnlargeCapacity(int)</see> or you can
    /// delete it with <see cref="MimeCache.Clear()">MimeCache.Clear()</see> if your application does not need it anymore.
    /// </para>
    /// </remarks>
    /// 
    /// <seealso cref="MimeType.FromFileName(ReadOnlySpan{char})"/>
    /// <seealso cref="MimeType.FromFileName(string?)"/>
    /// 
    /// <example>
    /// <para>
    /// Convert a file name into an Internet Media Type and get a file type extension from an internet media type:
    /// </para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FileExtensionExample.cs"/>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromFileName(ReadOnlySpan<char> fileName)
    {
#if NET461 || NETSTANDARD2_0
        return FromFileName(fileName.ToString());
#else
        ReadOnlySpan<char> ext = Path.GetExtension(fileName);
        fileName = ext.IsEmpty ? Path.GetFileName(fileName) : ext;

        return MimeCache.GetMimeType(fileName);
#endif
    }
}
