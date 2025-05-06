using Xunit;
using datastructures_project.HashTables;

public class SeparateChainingHashTableTests
{
    [Fact]
    public void Insert_And_Search_ShouldReturnTrue()
    {
        var hashTable = new SeparateChainingHashTable(5);
        hashTable.Insert(15);
        Assert.True(hashTable.Search(15));
    }

    [Fact]
    public void Delete_ShouldRemoveItem()
    {
        var hashTable = new SeparateChainingHashTable(5);
        hashTable.Insert(15);
        hashTable.Delete(15);
        Assert.False(hashTable.Search(15));
    }
}