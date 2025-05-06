using Xunit;
using datastructures_project.HashTables;

public class LinearProbingTests
{
    [Fact]
    public void Insert_And_Search_ShouldReturnTrue()
    {
        var hashTable = new LinearProbingHashTable(10);
        hashTable.Insert(15);

        // Assuming you have a Search method that returns true/false
        bool found = hashTable.Search(15);
        Assert.True(found);
    }

    [Fact]
    public void Delete_ShouldRemoveItem()
    {
        var hashTable = new LinearProbingHashTable(10);
        hashTable.Insert(15);
        hashTable.Delete(15);

        bool found = hashTable.Search(15);
        Assert.False(found);
    }
}