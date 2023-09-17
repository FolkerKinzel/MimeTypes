using System;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser()]
public class PassingSpansBench
{


    private int TwoSpansDirectly(ReadOnlySpan<char> span1, ReadOnlySpan<char> span2)
        => span1.Length + span2.Length;

    private int ThreeSpansDirectly(ReadOnlySpan<char> span1, ReadOnlySpan<char> span2, ReadOnlySpan<char> span3)
        => span1.Length + span2.Length + span3.Length;

    

    
}
