namespace tests.Search.Trie;

public class TrieTest
{
    [Fact]
    public void Trie_AddWord_SearchWord()
    {
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Add some words to trie
        trie.AddWord("test");
        trie.AddWord("testing");
        trie.AddWord("tester");
        trie.AddWord("testify");
        trie.AddWord("kapı");
        trie.AddWord("kapıdan");
        trie.AddWord("kaplamak");
        trie.AddWord("kapatmak");
        trie.AddWord("kitap");
        
        Assert.True(trie.SearchWord("test"));
        Assert.True(trie.SearchWord("testing"));
        Assert.True(trie.SearchWord("tester"));
        Assert.True(trie.SearchWord("testify"));
        Assert.True(trie.SearchWord("kapı"));
        Assert.True(trie.SearchWord("kapıdan"));
        Assert.True(trie.SearchWord("kaplamak"));
        Assert.True(trie.SearchWord("kapatmak"));
        Assert.True(trie.SearchWord("kitap"));
        Assert.False(trie.SearchWord("testa"));
        Assert.False(trie.SearchWord("kapat"));
    }
    
    [Fact]
    public void Trie_GetWords()
    {
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Add some words to trie
        trie.AddWord("test");
        trie.AddWord("testing");
        trie.AddWord("tester");
        trie.AddWord("testify");
        trie.AddWord("kapı");
        trie.AddWord("kapıdan");
        trie.AddWord("kaplamak");
        trie.AddWord("kapatmak");
        trie.AddWord("kitap");

        var words = trie.GetWords("te");
        
        Assert.Contains("test", words);
        Assert.Contains("testing", words);
        Assert.Contains("tester", words);
        Assert.Contains("testify", words);
        Assert.Equal(4, words.Count);

        words = trie.GetWords("testi");
        
        Assert.Contains("testify", words);
        Assert.Contains("testing", words);
        Assert.Equal(2, words.Count);
        
        words = trie.GetWords("kapı");
        
        Assert.Contains("kapı", words);
        Assert.Contains("kapıdan", words);
        Assert.Equal(2, words.Count);
        
        words = trie.GetWords("kap");
        
        Assert.Contains("kapı", words);
        Assert.Contains("kapıdan", words);
        Assert.Contains("kaplamak", words);
        Assert.Contains("kapatmak", words);
        Assert.Equal(4, words.Count);
        
        words = trie.GetWords("kitap");
        
        Assert.Contains("kitap", words);
        Assert.Equal(1, words.Count);
        
        words = trie.GetWords("kapatmakk");
        Assert.Empty(words);
    }
    
    [Fact]
    public void Trie_EmptyTrie()
    {
        var trie = new datastructures_project.Search.Trie.Trie();
        
        Assert.False(trie.SearchWord("test"));
        Assert.Empty(trie.GetWords("test"));
    }
    
    [Fact]
    public void Trie_EmptyString()
    {
        var trie = new datastructures_project.Search.Trie.Trie();
        
        trie.AddWord("");
        
        Assert.True(trie.SearchWord(""));
        Assert.Empty(trie.GetWords("test"));
    }

    [Fact]
    public void Trie_WildcardSearch()
    {
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Add some words to trie        
        string[] addedWords = [
            "test",
            "testing",
            "tester",
            "testify",
            "kapı",
            "kapıdan",
            "kaplamak",
            "kapatmak",
            "kitap",
            "kitaplık",
            "kitaplıklar",
            "kitaplıkta",
            "kalemlik",
            "kalem",
            "kalemler",
            "kalemlikler",
            "uzaklık",
            "uzak",
            "uzunluk",
            "uzun",
            "uzunluklar"
        ];
        
        foreach (var word in addedWords)
        {
            trie.AddWord(word);
        }
        
        // Test wildcard search
        var words = trie.WildcardSearch("te*t");
        Assert.Contains("test", words);
        Assert.Equal(1, words.Count);
        
        words = trie.WildcardSearch("test*");
        Assert.Contains("test", words);
        Assert.Contains("testing", words);
        Assert.Contains("tester", words);
        Assert.Contains("testify", words);
        Assert.Equal(4, words.Count);
        
        words = trie.WildcardSearch("*st*");
        Assert.Contains("test", words);
        Assert.Contains("testing", words);
        Assert.Contains("tester", words);
        Assert.Contains("testify", words);
        Assert.Equal(4, words.Count);

        words = trie.WildcardSearch("*lik");
        Assert.Contains("kalemlik", words);
        Assert.Equal(1, words.Count);
        
        words = trie.WildcardSearch("k*ap*");
        Assert.Contains("kapı", words);
        Assert.Contains("kapıdan", words);
        Assert.Contains("kaplamak", words);
        Assert.Contains("kapatmak", words);
        Assert.Contains("kitap", words);
        Assert.Contains("kitap", words);
        Assert.Contains("kitaplık", words);
        Assert.Contains("kitaplıklar", words);
        Assert.Contains("kitaplıkta", words);
        Assert.Equal(8, words.Count);
    }
    
    [Fact]
    public void Trie_WildcardSearch_EmptyTrie()
    {
        var trie = new datastructures_project.Search.Trie.Trie();
        
        Assert.Empty(trie.WildcardSearch("test"));
        Assert.Empty(trie.WildcardSearch("*"));
        Assert.Empty(trie.WildcardSearch("t*"));
        Assert.Empty(trie.WildcardSearch("*t"));
        Assert.Empty(trie.WildcardSearch("*test*"));
    }
    
    [Fact]
    public void Trie_LevenshteinSearch()
    {
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Add some words to trie
        string[] addedWords = [
            "test",
            "testing",
            "tester",
            "testify",
            "kapı",
            "kapıdan",
            "kitap",
            "kalemlik",
            "kalem",
            "kalemler",
        ];
        
        foreach (var word in addedWords)
        {
            trie.AddWord(word);
        }
        
        // Test Levenshtein search
        var words = trie.LevenshteinSearch("test", 3);
        Assert.Contains("test", words);
        Assert.Contains("testing", words);
        Assert.Contains("tester", words);
        Assert.Contains("testify", words);
        Assert.Equal(4, words.Count);
        
        words = trie.LevenshteinSearch("test", 2);
        Assert.Contains("test", words);
        Assert.Contains("tester", words);
        Assert.Equal(2, words.Count);
        
        words = trie.LevenshteinSearch("kapan", 2);
        Assert.Contains("kapı", words);
        Assert.Contains("kapıdan", words);
        Assert.Equal(2, words.Count);
        
        words = trie.LevenshteinSearch("kalemli", 3);
        Assert.Contains("kalemlik", words);
        Assert.Contains("kalem", words);
        Assert.Contains("kalemler", words);
        Assert.Equal(3, words.Count);
    }
    
    [Fact]
    public void Trie_WildcardSearch_EmptyString()
    {
        var trie = new datastructures_project.Search.Trie.Trie();
        
        trie.AddWord("");
        
        Assert.True(trie.SearchWord(""));
        Assert.Empty(trie.GetWords("test"));
        Assert.Empty(trie.WildcardSearch("test"));
    }

    [Fact]
    public void Trie_GetTokens()
    {
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Add some words to trie
        string[] addedWords = [
            "test",
            "testing",
            "tester",
            "testify",
            "kapı",
            "kapıdan",
            "kitap",
            "kalemlik",
            "kalem",
            "kalemler",
        ];
        
        foreach (var word in addedWords)
        {
            trie.AddWord(word);
        }
        
        // Test GetTokens
        string[] tokens = ["testif*","*al*", "kap*", "kalemli"];
        var newTokens = trie.GetTokens(tokens.ToList());
        Console.WriteLine(string.Join(", ", newTokens));
        
        Assert.Contains("testify", newTokens);
        Assert.Contains("kalem", newTokens);
        Assert.Contains("kalemlik", newTokens);
        Assert.Contains("kalemler", newTokens);
        Assert.Contains("kapı", newTokens);
        Assert.Contains("kapıdan", newTokens);
        Assert.Equal(2, newTokens.Count(t => t == "kalem"));
        Assert.Equal(2, newTokens.Count(t => t == "kalemlik"));
        Assert.Equal(2, newTokens.Count(t => t == "kalemler"));
        Assert.Equal(9, newTokens.Count);
    }
}