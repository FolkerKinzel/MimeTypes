﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Tests;

public static class HelperExtension
{
    public static IEnumerable<TSource> AsWeakEnumerable<TSource>(this IEnumerable<TSource> source) => source.EnumerateWeak().Cast<TSource>();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1510:Use ArgumentNullException throw helper", Justification = "<Pending>")]
    private static IEnumerable EnumerateWeak(this IEnumerable source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        foreach (object? o in source)
        {
            yield return o;
        }
    }


    public static int GetLinesCount(this string? source) 
    {
        int count = 0;
        if (source == null) { return count; }
        if (source.Length == 0) { return 1; }

        using var reader = new StringReader(source);
        while(reader.ReadLine() != null) { count++; }
        return count;
    }

    public static bool HasEmptyLine(this string? source)
    {
        if (source is null) { return false; }
        if (source.Length == 0) { return true; }

        using var reader = new StringReader(source);

        string? s;
        while ((s = reader.ReadLine()) != null)
        { 
            if(s.Length == 0)
            {
                return true;
            }
        }
        return false;
    }
}
