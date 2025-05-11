using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using datastructures_project.HashTables;
using System.Collections.Generic;
using BenchmarkDotNet.Engines;

namespace HashTableBenchmarks;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.ColdStart, launchCount: 25,
    warmupCount: 2)]
[MinColumn, MaxColumn, MeanColumn]
public class BTreeBenchmark
{
    private BTreeDictionary<int, int> _customTree;
    private int[] _keys;

    [Params(1000, 10000)] public int N;

    [GlobalSetup]
    public void Setup()
    {
        _customTree = new BTreeDictionary<int, int>();
        _keys = new int[N];

        var rand = new Random(42);
        for (int i = 0; i < N; i++)
        {
            int key = rand.Next();
            _keys[i] = key;
            _customTree.Add(key, i);
        }
    }

    [Benchmark]
    public void Benchmark_BTree_TryGetValue()
    {
        foreach (var key in _keys)
        {
            _customTree.TryGetValue(key, out _);
        }
    }

    [Benchmark]
    public void Benchmark_BTree_Add()
    {
        var table = new BTreeDictionary<int, int>();
        foreach (var key in _keys)
        {
            table.Add(key, key);
        }
    }
    
    [Benchmark]
    public void Benchmark_BTree_ContainsKey()
    {
        foreach (var key in _keys)
        {
            _customTree.ContainsKey(key);
        }
    }
    
    [Benchmark]
    public void Benchmark_BTree_AddRemove()
    {
        var table = new BTreeDictionary<int, int>();
        foreach (var key in _keys)
        {
            table.Add(key, key);
        }

        foreach (var key in _keys)
        {
            table.Remove(key);
        }
    }
}