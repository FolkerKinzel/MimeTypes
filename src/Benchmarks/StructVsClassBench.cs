using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;



[MemoryDiagnoser]
[BaselineColumn]
public class StructVsClassBench
{
    [Benchmark(Baseline = true)]
    public int TestStruct()
    {
        var dic = new TestDictionaryStruct()
        {
            new TestKvpStruct(){ Key="1" },
            new TestKvpStruct(){ Key="2" },
            new TestKvpStruct(){ Key="3" },
            //new TestKvpStruct(){ Key="4" },
            //new TestKvpStruct(){ Key="5" },
            //new TestKvpStruct(){ Key="6" },
            //new TestKvpStruct(){ Key="7" },
            //new TestKvpStruct(){ Key="8" },
            //new TestKvpStruct(){ Key="9" },
            //new TestKvpStruct(){ Key="0" },
        };

        int counter = 0;
        for (int i = 0; i < 3; i++)
        {
            var testKvp = dic[i];
            if(testKvp.Key != null) { counter++; }
        }

        return counter;
    }

    [Benchmark]
    public int TestTheClass()
    {
        var dic = new TestDictionaryClass()
        {
            new TestKvpClass(){ Key="1" },
            new TestKvpClass(){ Key="2" },
            new TestKvpClass(){ Key="3" },
            //new TestKvpClass(){ Key="4" },
            //new TestKvpClass(){ Key="5" },
            //new TestKvpClass(){ Key="6" },
            //new TestKvpClass(){ Key="7" },
            //new TestKvpClass(){ Key="8" },
            //new TestKvpClass(){ Key="9" },
            //new TestKvpClass(){ Key="0" },
        };

        int counter = 0;
        for (int i = 0; i < 3; i++)
        {
            var testKvp = dic[i];
            if (testKvp.Key != null) { counter++; }
        }

        return counter;
    }
}
