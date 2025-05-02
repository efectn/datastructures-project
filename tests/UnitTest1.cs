using datastructures_project.Search.Trie;

namespace tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var trie = new Trie();
        trie.AddWord("test");
        Assert.Equal(1, trie.GetWords("tes").Count);
    }
}