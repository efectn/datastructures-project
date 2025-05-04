using datastructures_project.Search.Index;
using datastructures_project.Search.Trie;
using Moq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace tests.Search.Index;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.ColdStart, launchCount: 25,
    warmupCount: 5)]
[MinColumn, MaxColumn, MeanColumn]

public class InvertedIndexBenchmark
{
    [Params(1000)] public int N;

    private InvertedIndex _invertedIndex;
    private List<string> _words;
    private Mock<ITrie> _trieMock;
    private int _nextDocId;

    [GlobalSetup]
    public void Setup()
    {
        _trieMock = new Mock<ITrie>();
        _trieMock.Setup(t => t.AddWord(It.IsAny<string>())).Verifiable();
        _trieMock.Setup(t => t.GetWords(It.IsAny<string>())).Returns(new List<string>());

        _invertedIndex = new InvertedIndex(_trieMock.Object);
        _words = Enumerable.Range(0, 100).Select(i => $"word{i}").ToList();

        for (int i = 0; i < 50; i++)
        {
            var wordsToAdd = _words.Take(10).ToArray(); // Fixed small size for consistency
            _invertedIndex.Add(i, wordsToAdd);
        }

        _nextDocId = 50;
    }

    [Benchmark]
    public void InvertedIndex_AddDocument()
    {
        var wordsToAdd = _words.OrderBy(_ => Random.Shared.Next()).Take(10).ToArray();
        _invertedIndex.Add(_nextDocId++, wordsToAdd);
    }

    [Benchmark]
    public void InvertedIndex_DocumentCount() => _invertedIndex.DocumentCount();

    [Benchmark]
    public void InvertedIndex_DocumentWordsCount() => _invertedIndex.DocumentWordsCount(0);

    [Benchmark]
    public void InvertedIndex_GetWords() => _invertedIndex.Trie.GetWords("word15");

    [Benchmark]
    public void InvertedIndex_WordDocuments() => _invertedIndex.WordDocuments("word15");

    [Benchmark]
    public void InvertedIndex_DocumentLength() => _invertedIndex.DocumentLength(0);

    [Benchmark]
    public void InvertedIndex_WordDocuments_Empty() => _invertedIndex.WordDocuments("nonexistent");

    [Benchmark]
    public void InvertedIndex_DocumentLength_Empty() => _invertedIndex.DocumentLength(99999);

    [Benchmark]
    public void InvertedIndex_GetWords_Empty() => _invertedIndex.Trie.GetWords("nonexistent");

    [Benchmark]
    public void InvertedIndex_DocumentIds() => _invertedIndex.DocumentIds();

    [Benchmark]
    public void InvertedIndex_Tokens() => _invertedIndex.Tokens(0);

    [Benchmark]
    public void InvertedIndex_AddRemove()
    {
        int tempDocId = _nextDocId++;
        var wordsToAdd = _words.Take(10).ToArray();
        _invertedIndex.Add(tempDocId, wordsToAdd);
        _invertedIndex.Remove(tempDocId);
    }
}
