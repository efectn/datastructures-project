using BenchmarkDotNet.Attributes;
using datastructures_project.Search.Trie;

[MemoryDiagnoser]
public class TrieBenchmark
{
    [Params(100, 1000, 10000)]
    public int N;

    private Trie _trie;
    private List<string> _words;
    
    [GlobalSetup]
    public void Setup()
    {
        _trie = new Trie();
        
        // Generate random words
        _words = new List<string>(N/2);
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        var random = new Random();

        for (int i = 0; i < N/2; i++)
        {
            int len = random.Next(4, 15);
            for (int j = 0; j < len; j++)
            {
                var charLoc = random.Next(chars.Length);
                _words.Add(chars[charLoc].ToString());
            }
        }
        
        foreach (var word in _words)
        {
            _trie.AddWord(word);
        }
        
        // Add a few words to the trie
        var knownWords = new List<string>
        {
            "test",
            "testing",
            "tester",
            "testify",
            "testament",
            "testable"
        };
        
        foreach (var word in knownWords)
        {
            _trie.AddWord(word);
        }
    }

    [Benchmark]
    public void AddWord()
    {
        var random = new Random();
        var res = random.Next(0, N/2);
        var newTrie = new Trie();
        newTrie.AddWord(_words[res]);
    }

    [Benchmark]
    public bool SearchWord() => _trie.SearchWord(_words[N / 2]);

    [Benchmark]
    public List<string> GetWordsWithPrefix() => _trie.GetWords("test");

    [Benchmark]
    public List<string> WildcardSearch() => _trie.WildcardSearch("te*t");

    [Benchmark]
    public List<string> LevenshteinSearch() => _trie.LevenshteinSearch("tes", 2);
}