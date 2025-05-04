using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using datastructures_project.Search.Index;
using datastructures_project.Search.Score;
using datastructures_project.Search.Trie;
using Moq;

namespace tests.Search.Score;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.ColdStart, launchCount: 25,
    warmupCount: 2)]
[MinColumn, MaxColumn, MeanColumn]
public class BM25Benchmark
{
    [Params(1000)] public int N;
    
    private BM25 _bm25;
    private string[] _queryTokens;

    [GlobalSetup]
    public void Setup()
    {
        var mockIndex = new Mock<IIndex>();
        var documents = new Dictionary<string, (int docId, int freq)[]>
        {
            { "search", [(1, 3), (2, 2)] },
            { "engine", [(1, 1), (3, 4)] }
        };

        // Configure mock behavior
        mockIndex.Setup(i => i.WordDocuments(It.IsAny<string>()))
            .Returns((string token) => documents.ContainsKey(token) ? documents[token] : Array.Empty<(int, int)>());

        mockIndex.Setup(i => i.DocumentCount()).Returns(5);
        
        mockIndex.Setup(i => i.DocumentLength(It.IsAny<int>()))
            .Returns((int docId) => docId switch
            {
                1 => 4,
                2 => 2,
                3 => 4,
                _ => 0
            });

        mockIndex.Setup(i => i.AverageDoclength).Returns(3.33);
        mockIndex.Setup(i => i.Trie).Returns(Mock.Of<ITrie>());

        _bm25 = new BM25(mockIndex.Object);
    }

    [Benchmark]
    public Dictionary<int, double> BM25_Calculate_Multiple()
    {
        return _bm25.Calculate(["search", "engine"]);
    }
    
    [Benchmark]
    public Dictionary<int, double> BM25_Calculate_Single()
    {
        return _bm25.Calculate(["search"]);
    }
}
