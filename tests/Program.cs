// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using tests.Search.Index;
using tests.Search.Score;

Console.WriteLine("Trie Benchmarks:");
BenchmarkRunner.Run<TrieBenchmark>();

Console.WriteLine("InvertedIndex Benchmarks:");
BenchmarkRunner.Run<InvertedIndexBenchmark>();

Console.WriteLine("ForwardIndex Benchmarks:");
BenchmarkRunner.Run<ForwardIndexBenchmark>();

Console.WriteLine("BM25 Benchmarks:");
BenchmarkRunner.Run<BM25Benchmark>();

Console.WriteLine("TF-IDF Benchmarks:");
BenchmarkRunner.Run<TFIDFBenchmark>();