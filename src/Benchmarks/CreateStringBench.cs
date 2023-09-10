using System;
using BenchmarkDotNet.Attributes;
using FolkerKinzel.Strings;

namespace Benchmarks;


[MemoryDiagnoser]
public class CreateStringBench
{
    private readonly string MEDIA_TYPE = "image";
    private readonly string SUB_TYPE = "png";



    [Benchmark]
    public string PlusSign() => (MEDIA_TYPE + '/' + SUB_TYPE).ToLowerInvariant();


    [Benchmark]
    public string Interpolated() => $"{MEDIA_TYPE}/{SUB_TYPE}".ToLowerInvariant();

    //[Benchmark]
    //public string StringConcat() => string.Concat(MEDIA_TYPE, "/", SUB_TYPE).ToLowerInvariant();

    [Benchmark]
    public string StringCreate()
    {
        return StaticStringMethod.Create(MEDIA_TYPE.Length + SUB_TYPE.Length + 1, (MEDIA_TYPE, SUB_TYPE),
            static (chars, tuple) =>
            {
                var media = tuple.MEDIA_TYPE.AsSpan();
                var sub = tuple.SUB_TYPE.AsSpan();
                media.CopyTo(chars);
                chars[media.Length] = '/';
                sub.CopyTo(chars.Slice(sub.Length + 1));
                chars.ToLowerInvariant();
            });
    }
}
