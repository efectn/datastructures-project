using Xunit;
using datastructures_project.HashTables;

public class DoubleHashingHashTableTests
{
    [Fact]
    public void Insert_And_Search_ShouldReturnTrue()
    {
        var hashTable = new DoubleHashingHashTable(10);
        hashTable.Insert(35);
        Assert.True(hashTable.Search(35));
    }

    [Fact]
    public void Delete_ShouldRemoveItem()
    {
        var hashTable = new DoubleHashingHashTable(10);
        hashTable.Insert(35);
        hashTable.Delete(35);
        Assert.False(hashTable.Search(35));
    }
}