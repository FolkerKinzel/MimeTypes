using System.Net;
using System.Text;
using BenchmarkDotNet.Attributes;
using FolkerKinzel.Strings;

namespace Benchmarks;

[MemoryDiagnoser]
public class UrlEncodingBench
{
    private const string INPUT = "a B 1+2 = %27 asdfghjkl äÖü";
    private const string CHARSET = "iso-8859-1";
    private readonly string _urlEncoded;

    public UrlEncodingBench()
    {
        var bytes = TextEncodingConverter.GetEncoding(CHARSET).GetBytes(INPUT);
        var encoded = WebUtility.UrlEncodeToBytes(bytes, 0, bytes.Length);
        _urlEncoded = Encoding.UTF8.GetString(encoded);
    }

    [Benchmark]
    public string StringBuilderExtension() => new StringBuilder().AppendUrlEncoded(INPUT).ToString();

    [Benchmark]
    public string StringBuilderExtension2() => new StringBuilder().AppendUrlEncoded2(INPUT).ToString();


    //[Benchmark]
    //public string UrlEncodingClassExtension()
    //{
    //    FolkerKinzel.MimeTypes.Intls.Parameters.Encodings.UrlEncoding.TryEncode(INPUT, out string? output);
    //    return new StringBuilder().Append(output).ToString();
    //}

    [Benchmark]
    public string? DecodeBenchmarkClass()
    {
        _ = Benchmarks.UrlEncoding.TryDecode(_urlEncoded, CHARSET, out var output);
        return output;
    }

    //[Benchmark]
    //public string? DecodeLibrary()
    //{
    //    _ = FolkerKinzel.MimeTypes.Intls.Parameters.Encodings.UrlEncodingHelper.TryDecode(_urlEncoded, CHARSET, out var output);
    //    return output;
    //}

}
