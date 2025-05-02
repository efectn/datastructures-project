using Moq;
using Microsoft.Extensions.Configuration;

namespace tests.Search.Tokenizer;

public class TokenizerTest
{
    private static string _createTempFile(string content)
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void Tokenizer_RemovesStopWords_WhenEnabled()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Search:Tokenizer:EnableStemmer"]).Returns("false");
        configMock.Setup(c => c["Search:Tokenizer:EnableStopWords"]).Returns("true");

        var stopWordsFile = _createTempFile("bu bir ve");
        var protectedWordsFile = _createTempFile("");

        var tokenizer = new datastructures_project.Search.Tokenizer.Tokenizer(configMock.Object, stopWordsFile, protectedWordsFile);
        var result = tokenizer.Tokenize("Bu bir test ve deneme");

        Assert.DoesNotContain("bu", result);
        Assert.DoesNotContain("bir", result);
        Assert.DoesNotContain("ve", result);
        Assert.Contains("test", result);
        Assert.Contains("deneme", result);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Tokenizer_StemsWords_WhenEnabled_AndNotProtected()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Search:Tokenizer:EnableStemmer"]).Returns("true");
        configMock.Setup(c => c["Search:Tokenizer:EnableStopWords"]).Returns("false");

        var stopWordsFile = _createTempFile("");
        var protectedWordsFile = _createTempFile("kitap");

        var tokenizer = new datastructures_project.Search.Tokenizer.Tokenizer(configMock.Object, stopWordsFile, protectedWordsFile);
        var text = "Kitaplar öğrenciler kitap masa";

        var result = tokenizer.Tokenize(text);

        Assert.Contains("kitap", result);
        Assert.Contains("öğrenci", result);
        Assert.Contains("mas", result);

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void Tokenizer_KeepsStopWords_WhenDisabled()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Search:Tokenizer:EnableStemmer"]).Returns("false");
        configMock.Setup(c => c["Search:Tokenizer:EnableStopWords"]).Returns("false");

        var stopWordsFile = _createTempFile("bu bir ve");
        var protectedWordsFile = _createTempFile("");

        var tokenizer = new datastructures_project.Search.Tokenizer.Tokenizer(configMock.Object, stopWordsFile, protectedWordsFile);
        var result = tokenizer.Tokenize("Bu bir test ve deneme");

        Assert.Contains("bu", result);
        Assert.Contains("bir", result);
        Assert.Contains("ve", result);
        Assert.Contains("test", result);
        Assert.Contains("deneme", result);

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Tokenizer_RemovesPunctuation()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Search:Tokenizer:EnableStemmer"]).Returns("false");
        configMock.Setup(c => c["Search:Tokenizer:EnableStopWords"]).Returns("false");

        var stopWordsFile = _createTempFile("");
        var protectedWordsFile = _createTempFile("");

        var tokenizer = new datastructures_project.Search.Tokenizer.Tokenizer(configMock.Object, stopWordsFile, protectedWordsFile);
        var result = tokenizer.Tokenize("Merhaba, dünya! Bu bir test.");

        Assert.Contains("merhaba", result);
        Assert.Contains("dünya", result);
        Assert.Contains("bu", result);
        Assert.Contains("bir", result);
        Assert.Contains("test", result);
        Assert.DoesNotContain("!", result);
        Assert.DoesNotContain(",", result);
        Assert.DoesNotContain(".", result);

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void Tokenizer_EmptyInput_ReturnsEmptyList()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Search:Tokenizer:EnableStemmer"]).Returns("true");
        configMock.Setup(c => c["Search:Tokenizer:EnableStopWords"]).Returns("true");

        var stopWordsFile = _createTempFile("");
        var protectedWordsFile = _createTempFile("");

        var tokenizer = new datastructures_project.Search.Tokenizer.Tokenizer(configMock.Object, stopWordsFile, protectedWordsFile);
        var result = tokenizer.Tokenize("    ");

        Assert.Empty(result);
    }
}