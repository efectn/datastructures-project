using datastructures_project.Search.Index;
using datastructures_project.Search.Trie;
using Moq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace tests.Search.Index;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.ColdStart, launchCount: 25,
    warmupCount: 2)]
[MinColumn, MaxColumn, MeanColumn]

public class ForwardIndexBenchmark
{
    [Params(1000)] public int N;

    private ForwardIndex _forwardIndex;
    private List<string> _words;
    private Mock<ITrie> _trieMock;
    private int _nextDocId;

    [GlobalSetup]
    public void Setup()
    {
        _trieMock = new Mock<ITrie>();
        _trieMock.Setup(t => t.AddWord(It.IsAny<string>())).Verifiable();
        _trieMock.Setup(t => t.GetWords(It.IsAny<string>())).Returns(new List<string>());

        _forwardIndex = new ForwardIndex(_trieMock.Object);
        _words = Enumerable.Range(0, 100).Select(i => $"word{i}").ToList();

        for (int i = 0; i < 50; i++)
        {
            var wordsToAdd = _words.Take(10).ToArray(); // Fixed small size for consistency
            _forwardIndex.Add(i, wordsToAdd);
        }

        _nextDocId = 50;
    }

    [Benchmark]
    public void ForwardIndex_AddDocument()
    {
        var wordsToAdd = _words.OrderBy(_ => Random.Shared.Next()).Take(10).ToArray();
        _forwardIndex.Add(_nextDocId++, wordsToAdd);
    }

    [Benchmark]
    public void ForwardIndex_DocumentCount() => _forwardIndex.DocumentCount();

    [Benchmark]
    public void ForwardIndex_DocumentWordsCount() => _forwardIndex.DocumentWordsCount(0);

    [Benchmark]
    public void ForwardIndex_GetWords() => _forwardIndex.Trie.GetWords("word15");

    [Benchmark]
    public void ForwardIndex_WordDocuments() => _forwardIndex.WordDocuments("word15");

    [Benchmark]
    public void ForwardIndex_DocumentLength() => _forwardIndex.DocumentLength(0);

    [Benchmark]
    public void ForwardIndex_WordDocuments_Empty() => _forwardIndex.WordDocuments("nonexistent");

    [Benchmark]
    public void ForwardIndex_DocumentLength_Empty() => _forwardIndex.DocumentLength(99999);

    [Benchmark]
    public void ForwardIndex_GetWords_Empty() => _forwardIndex.Trie.GetWords("nonexistent");

    [Benchmark]
    public void ForwardIndex_DocumentIds() => _forwardIndex.DocumentIds();

    [Benchmark]
    public void ForwardIndex_Tokens() => _forwardIndex.Tokens(0);

    [Benchmark]
    public void ForwardIndex_AddRemove()
    {
        int tempDocId = _nextDocId++;
        var wordsToAdd = _words.Take(10).ToArray();
        _forwardIndex.Add(tempDocId, wordsToAdd);
        _forwardIndex.Remove(tempDocId);
    }
}
