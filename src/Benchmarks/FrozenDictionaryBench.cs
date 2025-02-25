using BenchmarkDotNet.Attributes;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace Benchmarks;

[MemoryDiagnoser]
public class FrozenDictionaryBench
{
    private static readonly FrozenDictionary<char, (int, int)> _frozenChar = new (char Key, int ItemA, int ItemB)[]
    {
    ('a', 0,0),
    ('b', 0,0),
    ('c', 0,0),
    ('d', 0,0),
    ('e', 0,0),
    ('f', 0,0),
    ('g', 0,0),
    ('h', 0,0),
    ('i', 0,0),
    ('j', 0,0),
    ('k', 0,0),
    ('l', 0,0),
    ('m', 0,0),
    ('p', 0,0),
    ('q', 0,0),
    ('r', 0,0),
    ('s', 0,0),
    ('t', 0,0),
    ('u', 0,0),
    ('v', 0,0),
    ('w', 0,0),
    ('x', 0,0),
    ('y', 0,0),
    ('z', 0,0),
    ('0', 0,0),
    ('1', 0,0),
    ('2', 0,0),
    ('3', 0,0),
    ('4', 0,0),
    ('5', 0,0),
    ('6', 0,0),
    ('7', 0,0),
    }.ToFrozenDictionary(x => x.Key, x => (x.ItemA, x.ItemB));

    private static readonly Dictionary<char, (int, int)> _dicChar = _frozenChar.ToDictionary();


    private static readonly FrozenDictionary<string, (int, int)> _frozenStr = new (string Key, int ItemA, int ItemB)[]
    {
    ("XXXXXXXXXXXXXXXXXXXXa", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXb", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXc", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXd", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXe", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXf", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXg", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXh", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXi", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXj", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXk", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXl", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXm", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXp", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXq", 0,0),
    ("XXXXXXXXXXXXXXXXXXXXr", 0,0)
    }.ToFrozenDictionary(x => x.Key, x => (x.ItemA, x.ItemB));

    private static readonly Dictionary<string, (int, int)> _dicStr = _frozenStr.ToDictionary();


    [Benchmark]
    public bool GetFrozenChar() => _frozenChar.TryGetValue('u', out _);

    [Benchmark]
    public bool GetDicChar() => _dicChar.TryGetValue('u', out _);


    [Benchmark]
    public bool GetFrozenStr() => _frozenStr.TryGetValue("XXXXXXXXXXXXXXXXXXXXi", out _);

    [Benchmark]
    public bool GetDicStr() => _dicStr.TryGetValue("XXXXXXXXXXXXXXXXXXXXi", out _);

}
