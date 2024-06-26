﻿using BenchmarkDotNet.Attributes;
using System;
using System.Text;

namespace Benchmarks;

[MemoryDiagnoser]
public class UrisBench
{
    private const string INPUT = "eeeeeeeeeeeeeeeeeeeeEEEEEEEEEEEEEEEEEEEE";
    private const string LONG_STRING = "abcdefghijklmnopqrstUVWXYZ0123456789abcdefghijklmnopqrstUVWXYZ0123456789";

    private readonly StringBuilder _builder = new(INPUT.Length);
    //private const string TEST = "test";

    //private readonly DataUrl _dataUrlText1;
    //private readonly DataUrl _dataUrlText2;

    public UrisBench()
    {
        //const string data = "Märchenbücher";
        //const string isoEncoding = "iso-8859-1";

#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
        //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        //string s = $"data:;charset={isoEncoding};base64,{Convert.ToBase64String(Encoding.GetEncoding(isoEncoding).GetBytes(data))}";

        //_dataUrlText1 = DataUrl.Parse(s);
        //_dataUrlText2 = DataUrl.Parse(DataUrl.FromText(data));
    }

    [Benchmark(Baseline = true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0302:Simplify collection initialization", Justification = "<Pending>")]
    public bool ContainsTSpecials()
    {
        // RFC 2045 Section 5.1 "tspecials"
        // Calling MemoryExtensions directly to avoid allocation.
        // This method is much slower than string.IndexOfAny(char[]) but doesn't allocate.
        ReadOnlySpan<char> span = LONG_STRING.AsSpan();
        return MemoryExtensions.IndexOfAny(span,
            stackalloc char[] { '(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"', '/', '[', ']', '?', '=' }) != -1;
    }

    [Benchmark]
    public bool Switch()
    {
        ReadOnlySpan<char> span = LONG_STRING.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case '(':
                case ')':
                case '<':
                case '>':
                case '@':
                case ',':
                case ';':
                case ':':
                case '\\':
                case '\"':
                case '/':
                case '[':
                case ']':
                case '?':
                case '=':
                    return true;
                default:
                    break;
            }
        }
        return false;
    }

    //[Benchmark]
    //public StringBuilder ToLowerBench() => _builder.Clear().Append(INPUT).ToLowerInvariant();

    //[Benchmark]
    //public StringBuilder AppendStackallock()
    //{
    //    return _builder.Append(stackalloc char[] { 't', 'e', 's', 't' }).Clear();
    //}

    //[Benchmark]
    //public StringBuilder AppendString() => _builder.Append("test").Clear();

    //[Benchmark]
    //public bool StartsWithString1()
    //    => TEST.AsSpan().StartsWith("test", StringComparison.OrdinalIgnoreCase);

    //[Benchmark]
    //public bool StartsWithString2()
    //    => TEST.StartsWith("test", StringComparison.OrdinalIgnoreCase);


    //[Benchmark]
    //public bool StartsWithStackallock()
    //    => TEST.AsSpan().StartsWith(stackalloc char[] { 't', 'e', 's', 't' }, StringComparison.OrdinalIgnoreCase);


    //[Benchmark]
    //public bool EqualsBench()
    //{
    //    return _dataUrlText1.Equals(_dataUrlText2);
    //}

    //[Benchmark]
    //public static bool ReadOnlyMemoryByValue()
    //{
    //    var memory = default(ReadOnlyMemory<char>);
    //    return DoReadOnlyMemoryByValue(memory);
    //}

    //[Benchmark]
    //public static bool ReadOnlyMemoryByIn()
    //{
    //    var memory = default(ReadOnlyMemory<char>);
    //    return DoReadOnlyMemoryByIn(ref memory);
    //}

    //[MethodImpl(MethodImplOptions.NoInlining)]
    //private static bool DoReadOnlyMemoryByValue(ReadOnlyMemory<char> largeStruct) => largeStruct.IsEmpty;

    //[MethodImpl(MethodImplOptions.NoInlining)]
    //private static bool DoReadOnlyMemoryByIn(ref ReadOnlyMemory<char> largeStruct) => largeStruct.IsEmpty;

    //[Benchmark]
    //public bool DataUrlByValue()
    //{
    //    var dataUrl = default(DataUrl);
    //    return DoDataUrlByValue(dataUrl);
    //}

    //[Benchmark]
    //public bool DataUrlByIn()
    //{
    //    var dataUrl = default(DataUrl);
    //    return DoDataUrlByIn(in dataUrl);
    //}

    //[MethodImpl(MethodImplOptions.NoInlining)]
    //private static bool DoDataUrlByValue(DataUrl largeStruct) => largeStruct.IsEmpty;

    //[MethodImpl(MethodImplOptions.NoInlining)]
    //private static bool DoDataUrlByIn(in DataUrl largeStruct) => largeStruct.IsEmpty;

    //[Benchmark]
    //public bool DataUrlEqualsByValue()
    //{
    //    var dataUrl = default(DataUrl);
    //    return dataUrl.Equals(dataUrl);
    //}

    //[Benchmark]
    //public bool DataUrlEqualsByIn()
    //{
    //    var dataUrl = default(DataUrl);
    //    return dataUrl.Equals(in dataUrl);
    //}




    //[Benchmark]
    //public bool MimeTypeByValue() => DoMimeTypeByValue(MimeType.Empty);

    //[Benchmark]
    //public bool MimeTypeByIn() => DoMimeTypeByIn(MimeType.Empty);

    //[MethodImpl(MethodImplOptions.NoInlining)]
    //private static bool DoMimeTypeByValue(MimeType largeStruct) => largeStruct.IsEmpty;

    //[MethodImpl(MethodImplOptions.NoInlining)]
    //private static bool DoMimeTypeByIn(in MimeType largeStruct) => largeStruct.IsEmpty;





}
