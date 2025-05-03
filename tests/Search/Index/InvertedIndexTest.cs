namespace tests.Search.Index;

public class InvertedIndexTest
{
    [Fact]
    public void InvertedIndex_Add()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the inverted index
        var invertedIndex = new datastructures_project.Search.Index.InvertedIndex(trie);
        
        // Add some documents to the inverted index
        invertedIndex.Add(1, new[] {"hello", "world"});
        invertedIndex.Add(2, new[] {"hello", "everyone"});
        invertedIndex.Add(3, new[] {"goodbye", "world"});
        invertedIndex.Add(4, new[] {"hello", "john", "doe", "world", "hello"});
        
        // Check the document count
        Assert.Equal(4, invertedIndex.DocumentCount());
        
        // Check the document words count
        Assert.Equal(2, invertedIndex.DocumentWordsCount(1));
        Assert.Equal(2, invertedIndex.DocumentWordsCount(2));
        Assert.Equal(2, invertedIndex.DocumentWordsCount(3));
        Assert.Equal(5, invertedIndex.DocumentWordsCount(4));
        
        // Check the average document length
        Assert.Equal(14.25, invertedIndex.AverageDoclength, 2);
    }

    [Fact]
    public void InvertedIndex_DocumentLength()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the inverted index
        var invertedIndex = new datastructures_project.Search.Index.InvertedIndex(trie);
        
        // Add some documents to the inverted index
        invertedIndex.Add(1, new[] {"hello", "world"});
        invertedIndex.Add(2, new[] {"hello", "everyone"});
        invertedIndex.Add(3, new[] {"hello", "hello", "everyone", "everyone"});

        // Check the document length
        Assert.Equal(10, invertedIndex.DocumentLength(1));
        Assert.Equal(13, invertedIndex.DocumentLength(2));
        Assert.Equal(26, invertedIndex.DocumentLength(3));
    }

    [Fact]
    public void InvertedIndex_DocumentIds()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the inverted index
        var invertedIndex = new datastructures_project.Search.Index.InvertedIndex(trie);
        
        // Add some documents to the inverted index
        invertedIndex.Add(1, new[] {"hello", "world"});
        invertedIndex.Add(2, new[] {"hello", "everyone"});
        invertedIndex.Add(3, new[] {"goodbye", "world"});
        
        // Check the document IDs for a word
        Assert.Contains((1, 1), invertedIndex.Index["hello"]);
        Assert.Contains((2, 1), invertedIndex.Index["hello"]);
        Assert.Contains((3, 1), invertedIndex.Index["world"]);
        Assert.Contains((1, 1), invertedIndex.Index["world"]);
        Assert.Contains((2, 1), invertedIndex.Index["everyone"]);
        Assert.Contains((3, 1), invertedIndex.Index["goodbye"]);

        var ids = invertedIndex.DocumentIds();
        Assert.Contains(1, ids);
        Assert.Contains(2, ids);
        Assert.Contains(3, ids);
        
        Assert.Equal(3, ids.Count);
    }

    [Fact]
    public void InvertedIndex_Tokens()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the inverted index
        var invertedIndex = new datastructures_project.Search.Index.InvertedIndex(trie);
        
        // Add some documents to the inverted index
        invertedIndex.Add(1, new[] {"hello", "world", "hello", "test", "world"});
        invertedIndex.Add(2, new[] {"hello", "everyone"});
        invertedIndex.Add(3, new[] {"goodbye", "world"});
        
        // Check the tokens
        var tokens = invertedIndex.Tokens(1);
        Assert.Contains("hello", tokens);
        Assert.Contains("world", tokens);
        Assert.Contains("test", tokens);
        Assert.Equal(2, tokens.Count(t => t == "hello"));
        Assert.Equal(2, tokens.Count(t => t == "world"));
        
        Assert.Equal(5, tokens.Count);
        
        tokens = invertedIndex.Tokens(2);
        Assert.Contains("hello", tokens);
        Assert.Contains("everyone", tokens);
        
        Assert.Equal(2, tokens.Count);
        
        tokens = invertedIndex.Tokens(3);
        Assert.Contains("goodbye", tokens);
        Assert.Contains("world", tokens);
        
        Assert.Equal(2, tokens.Count);
    }
    
    [Fact]
    public void InvertedIndex_Remove_AverageDocLength()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the inverted index
        var invertedIndex = new datastructures_project.Search.Index.InvertedIndex(trie);
        
        Assert.Equal(0, invertedIndex.AverageDoclength);
        
        // Add some documents to the inverted index
        invertedIndex.Add(1, new[] {"hello", "world"});
        Assert.Equal(10.0, invertedIndex.AverageDoclength, 0.1);
        
        invertedIndex.Add(2, new[] {"hello", "everyone", "hello", "everyone"});
        Assert.Equal(18, invertedIndex.AverageDoclength, 0.1);
        
        invertedIndex.Add(3, new[] {"goodbye", "world", "test"});
        Assert.Equal(17.33, invertedIndex.AverageDoclength, 0.1);
        
        // Remove non-existing document
        invertedIndex.Remove(15);
        Assert.Equal(17.33, invertedIndex.AverageDoclength, 0.1);
        
        // Remove a document
        invertedIndex.Remove(1);
        Assert.Equal(21, invertedIndex.AverageDoclength, 0.1);
        
        // Remove another document
        invertedIndex.Remove(2);
        Assert.Equal(16.0, invertedIndex.AverageDoclength, 0.1);
        
        // Remove the last document
        invertedIndex.Remove(3);
        Assert.Equal(0.0, invertedIndex.AverageDoclength, 0.1);
        
        // Remove the last document again
        invertedIndex.Remove(3);
        Assert.Equal(0.0, invertedIndex.AverageDoclength, 0.1);
    }

    [Fact]
    public void InvertedIndex_Trie_WordsAdded()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the inverted index
        var invertedIndex = new datastructures_project.Search.Index.InvertedIndex(trie);
        
        // Add some documents to the inverted index
        invertedIndex.Add(1, new[] {"hello", "world"});
        invertedIndex.Add(2, new[] {"hello", "everyone"});
        invertedIndex.Add(3, new[] {"goodbye", "world"});
        
        // Check the words in the trie
        Assert.Contains("hello", trie.GetWords("h"));
        Assert.Contains("world", trie.GetWords("w"));
        Assert.Contains("everyone", trie.GetWords("e"));
        Assert.Contains("goodbye", trie.GetWords("g"));
        
        Assert.DoesNotContain("test", trie.GetWords("t"));
        Assert.DoesNotContain("everyone", trie.GetWords("g"));
        
        Assert.Equal(1, trie.GetWords("h").Count);
        Assert.Equal(1, trie.GetWords("w").Count);
        Assert.Equal(1, trie.GetWords("e").Count);
    }
    
    [Fact]
    public void InvertedIndex_EmptyIndex()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the inverted index
        var invertedIndex = new datastructures_project.Search.Index.InvertedIndex(trie);
        
        // Check the document count
        Assert.Equal(0, invertedIndex.DocumentCount());
        
        // Check the average document length
        Assert.Equal(0, invertedIndex.AverageDoclength);
        
        // Check the document length
        Assert.Equal(0, invertedIndex.DocumentLength(1));
    }
}