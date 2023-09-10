﻿using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmarks;

internal class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnötige Zuweisung eines Werts.", Justification = "<Ausstehend>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Nicht verwendete Parameter entfernen", Justification = "<Ausstehend>")]
    private static void Main(string[] args)
    {
        StringBuilderExtension.WriteEncodes();
        //string s = new CreateStringBench().StringCreate();
        // Summary summary = BenchmarkRunner.Run<CreateStringBench>();
            //ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
    }
}
