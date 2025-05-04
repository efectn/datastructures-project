// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using tests.Search.Index;

Console.WriteLine("Trie Benchmarks:");
BenchmarkRunner.Run<TrieBenchmark>();

Console.WriteLine("InvertedIndex Benchmarks:");
BenchmarkRunner.Run<InvertedIndexBenchmark>();

Console.WriteLine("ForwardIndex Benchmarks:");
BenchmarkRunner.Run<ForwardIndexBenchmark>();