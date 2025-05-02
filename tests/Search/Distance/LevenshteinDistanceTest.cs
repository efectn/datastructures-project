using datastructures_project.Search.Distance;

namespace tests.Search.Distance;

public class LevenshteinDistanceTest
{
    [Fact]
    public void LevenshteinDistance_SameString()
    {
        Assert.Equal(0, LevenshteinDistance.Distance("test", "test"));
    }

    [Fact]
    public void LevenshteinDistance_EmptyString()
    {
        Assert.Equal(4, LevenshteinDistance.Distance("", "test"));
        Assert.Equal(4, LevenshteinDistance.Distance("test", ""));
    }

    [Fact]
    public void LevenshteinDistance_OneDifference()
    {
        Assert.Equal(1, LevenshteinDistance.Distance("test", "tent"));
        Assert.Equal(1, LevenshteinDistance.Distance("test", "tests"));
        Assert.Equal(1, LevenshteinDistance.Distance("test", "tast"));
    }

    [Fact]
    public void LevenshteinDistance_CaseSensitive()
    {
        Assert.Equal(1, LevenshteinDistance.Distance("Test", "test"));
    }

    [Fact]
    public void LevenshteinDistance_DifferentStrings()
    {
        Assert.Equal(4, LevenshteinDistance.Distance("test", "abcd"));
        Assert.Equal(3, LevenshteinDistance.Distance("kitten", "sitting"));
        Assert.Equal(2, LevenshteinDistance.Distance("flaw", "lawn"));
    }

    [Fact]
    public void LevenshteinDistance_Substring()
    {
        Assert.Equal(3, LevenshteinDistance.Distance("testing", "test"));
        Assert.Equal(3, LevenshteinDistance.Distance("test", "testing"));
        Assert.Equal(1, LevenshteinDistance.Distance("test", "tes"));
    }

    [Fact]
    public void LevenshteinDistance_Space()
    {
        Assert.Equal(1, LevenshteinDistance.Distance("hello world", "helloworld"));
    }

    [Fact]
    public void LevenshteinDistance_Typo()
    {
        Assert.Equal(2, LevenshteinDistance.Distance("recieve", "receive"));
        Assert.Equal(1, LevenshteinDistance.Distance("recieve", "recievee"));
    }

    [Fact]
    public void LevenshteinDistance_LongStrings()
    {
        string a = new string('a', 100);
        string b = new string('a', 99) + 'b';
        Assert.Equal(1, LevenshteinDistance.Distance(a, b));
    }
    
    [Fact]
    public void LevenshteinDistance_SingleCharacterStrings()
    {
        Assert.Equal(1, LevenshteinDistance.Distance("a", "b"));
        Assert.Equal(1, LevenshteinDistance.Distance("", "a"));
    }

    [Fact]
    public void LevenshteinDistance_BothEmptyStrings_ReturnsZero()
    {
        Assert.Equal(0, LevenshteinDistance.Distance("", ""));
    }
}