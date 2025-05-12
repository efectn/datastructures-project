// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using datastructures_project.HashTables;
using HashTableBenchmarks;
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

Console.WriteLine("Linear Probing Hash Table Benchmarks:");
BenchmarkRunner.Run<LinearProbingHashTableBenchmark>();

Console.WriteLine("Quadratic Probing Hash Table Benchmarks:");
BenchmarkRunner.Run<QuadraticProbingHashTableBenchmark>();

Console.WriteLine("Separate Chaining Hash Table Benchmarks:");
BenchmarkRunner.Run<SeparateChainingHashTableBenchmark>();

Console.WriteLine("Double Hashing Hash Table Benchmarks:");
BenchmarkRunner.Run<DoubleHashingHashTableBenchmark>();

Console.WriteLine("AVL Tree Benchmarks:");
BenchmarkRunner.Run<AVLTreeBenchmark>();

Console.WriteLine("Dictionary Benchmarks:");
BenchmarkRunner.Run<DictionaryBenchmark>();


Console.WriteLine("BTree Benchmarks:");
BenchmarkRunner.Run<BTreeBenchmark>();

Console.WriteLine("Red Black Tree Benchmarks:");
BenchmarkRunner.Run<RedBlackTreeBenchmark>();