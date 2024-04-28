using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmarks;

internal class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnötige Zuweisung eines Werts.", Justification = "<Ausstehend>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Nicht verwendete Parameter entfernen", Justification = "<Ausstehend>")]
    private static void Main(string[] args)
    {
        //var o = new UrlEncodingBench();
        //string encoded1 = o.StringBuilderExtension();
        //string encoded1b = o.StringBuilderExtension2();

        //string encoded2 = o.UrlEncodingClassExtension();
        //string? decoded1 = o.DecodeBenchmarkClass();
        //string? decoded2 = o.DecodeLibrary();

        //StringBuilderExtension.WriteEncodes();
        //string s = new CreateStringBench().StringCreate();
        Summary summary = BenchmarkRunner.Run<CreateStringBench>();
        //ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
    }
}
