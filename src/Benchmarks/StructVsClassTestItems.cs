using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks;

[StructLayout(LayoutKind.Auto)]
public readonly struct TestKvpStruct
{
    public string Key { get; init; }
    public string? Value { get; init; }
    public string? Language { get; init; }
}

public class TestDictionaryStruct : KeyedCollection<string, TestKvpStruct>
{
    /// <summary>
    /// Initializes a new <see cref="MimeTypeParameterModelDictionary"/> object.
    /// </summary>
    public TestDictionaryStruct() : base(StringComparer.OrdinalIgnoreCase, -1) { }


    /// <inheritdoc/>
    protected override string GetKeyForItem(TestKvpStruct item) => item.Key;
}

public class TestKvpClass
{
    public string Key { get; init; } = "";
    public string? Value { get; init; }
    public string? Language { get; init; }
}

public class TestDictionaryClass : KeyedCollection<string, TestKvpClass>
{
    /// <summary>
    /// Initializes a new <see cref="MimeTypeParameterModelDictionary"/> object.
    /// </summary>
    public TestDictionaryClass() : base(StringComparer.OrdinalIgnoreCase, -1) { }


    /// <inheritdoc/>
    protected override string GetKeyForItem(TestKvpClass item) => item.Key;
}
