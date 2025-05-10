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
public class DoubleHashingHashTableBenchmark
{
    private DoubleHashingHashTable<int, int> _customHashTable;
    private int[] _keys;

    [Params(1000, 10000)] public int N;

    [GlobalSetup]
    public void Setup()
    {
        _customHashTable = new DoubleHashingHashTable<int, int>();
        _keys = new int[N];

        var rand = new Random(42);
        for (int i = 0; i < N; i++)
        {
            int key = rand.Next();
            _keys[i] = key;
            _customHashTable.Add(key, i);
        }
    }

    [Benchmark]
    public void Benchmark_DoubleHashing_TryGetValue()
    {
        foreach (var key in _keys)
        {
            _customHashTable.TryGetValue(key, out _);
        }
    }

    [Benchmark]
    public void Benchmark_DoubleHashing_Add()
    {
        var table = new DoubleHashingHashTable<int, int>();
        foreach (var key in _keys)
        {
            table.Add(key, key);
        }
    }
    
    [Benchmark]
    public void Benchmark_DoubleHashing_ContainsKey()
    {
        foreach (var key in _keys)
        {
            _customHashTable.ContainsKey(key);
        }
    }
    
    [Benchmark]
    public void Benchmark_DoubleHashing_AddRemove()
    {
        var table = new DoubleHashingHashTable<int, int>();
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