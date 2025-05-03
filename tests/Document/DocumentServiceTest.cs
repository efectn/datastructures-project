using datastructures_project.Database.Repository;
using datastructures_project.Document;
using datastructures_project.Search.Index;
using datastructures_project.Search.Tokenizer;
using Moq;

namespace tests.Document;

public class DocumentServiceTests
{
    private readonly Mock<IDocumentRepository> _repoMock;
    private readonly Mock<IIndex> _indexMock;
    private readonly Mock<ITokenizer> _tokenizerMock;
    private readonly DocumentService _service;

    public DocumentServiceTests()
    {
        _repoMock = new Mock<IDocumentRepository>();
        _indexMock = new Mock<IIndex>();
        _tokenizerMock = new Mock<ITokenizer>();

        // Create default mock objects
        _repoMock.Setup(r => r.GetAllDocuments()).Returns(new List<datastructures_project.Database.Model.Document>());
        _tokenizerMock.Setup(t => t.Tokenize(It.IsAny<string>())).Returns(new List<string>());

        _service = new DocumentService(_indexMock.Object, _tokenizerMock.Object, _repoMock.Object);
    }

    [Fact]
    public void DocumentService_AddDocument()
    {
        var doc = new datastructures_project.Database.Model.Document("Title", "http://url", "desc") { Id = 1 };
        _repoMock.Setup(r => r.CreateDocument(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((doc, 1));
        _tokenizerMock.Setup(t => t.Tokenize("Title")).Returns(new List<string> { "title" });

        _service.AddDocument("Title", "http://url", "desc");

        _repoMock.Verify(r => r.CreateDocument("Title", "http://url", "desc"), Times.Once);
        _indexMock.Verify(i => i.Add(1, It.Is<string[]>(s => s.Length == 1 && s[0] == "title")), Times.Once);
    }

    [Fact]
    public void DocumentService_AddDocument_Fail()
    {
        _repoMock.Setup(r => r.CreateDocument(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((null, 0));

        Assert.Throws<ArgumentException>(() =>
            _service.AddDocument("Title", "http://url", "desc"));
    }

    [Fact]
    public void DocumentService_RemoveDocument()
    {
        _repoMock.Setup(r => r.RemoveDocument(1)).Returns(1);

        _service.RemoveDocument(1);

        _repoMock.Verify(r => r.RemoveDocument(1), Times.Once);
        _indexMock.Verify(i => i.Remove(1), Times.Once);
    }

    [Fact]
    public void DocumentService_RemoveDocument_Fail()
    {
        _repoMock.Setup(r => r.RemoveDocument(It.IsAny<int>())).Returns(0);
        Assert.Throws<ArgumentException>(() => _service.RemoveDocument(999));
    }

    [Fact]
    public void DocumentService_GetDocument()
    {
        var doc = new datastructures_project.Database.Model.Document("Test", "http://url", "desc") { Id = 5 };
        _repoMock.Setup(r => r.GetDocumentById(5)).Returns(doc);

        var result = _service.GetDocument(5);

        Assert.Equal(5, result.Id);
        Assert.Equal("Test", result.Title);
    }

    [Fact]
    public void DocumentService_GetDocument_Fail()
    {
        _repoMock.Setup(r => r.GetDocumentById(It.IsAny<int>()))
            .Returns((datastructures_project.Database.Model.Document)null);
        Assert.Throws<ArgumentException>(() => _service.GetDocument(123));
    }

    [Fact]
    public void DocumentService_GetAllDocuments()
    {
        var docs = new List<datastructures_project.Database.Model.Document>
        {
            new("A", "http://url1", "d1"),
            new("B", "http://url2", "d2")
        };
        _repoMock.Setup(r => r.GetAllDocuments()).Returns(docs);

        var result = _service.GetAllDocuments();

        Assert.Equal(2, result.Count);
    }
}