public class QuadraticProbingHashTableTests
{
    [Fact]
    public void Insert_And_Search_ShouldReturnTrue()
    {
        var hashTable = new QuadraticProbingHashTable(10);
        hashTable.Insert(25);
        Assert.True(hashTable.Search(25));
    }

    [Fact]
    public void Delete_ShouldRemoveItem()
    {
        var hashTable = new QuadraticProbingHashTable(10);
        hashTable.Insert(25);
        hashTable.Delete(25);
        Assert.False(hashTable.Search(25));
    }
}