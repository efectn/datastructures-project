using datastructures_project.Database.Repository;
using datastructures_project.Search.Index;
using datastructures_project.Search.Tokenizer;

namespace datastructures_project.Document;

public class DocumentService : IDocumentService
{
    private IDocumentRepository _documentRepository;
    private readonly IIndex[] _index;
    private readonly ITokenizer _tokenizer;

    public DocumentService(IIndex[] index, ITokenizer tokenizer, IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
        _index = index;
        _tokenizer = tokenizer;
        
        // Add previous documents to index
        var documents = documentRepository.GetAllDocuments();
        foreach (var document in documents)
        {
            var tokens = tokenizer.Tokenize(document.Title);
            foreach (var i in index)
            {
                i.Add(document.Id, tokens.ToArray());
            }
        }
    }

    public void AddDocument(string title, string url, string description)
    {
        var document = new Database.Model.Document(title, url, description);
        var (doc, rows) = _documentRepository.CreateDocument(title, url, description);
        if (rows == 0)
        {
            throw new ArgumentException("Document with this id already exists");
        }
        
        // Add the document to the index
        foreach (var i in _index)
        {
            i.Add(doc.Id, _tokenizer.Tokenize(document.Title).ToArray());
        }
    }

    public void RemoveDocument(int id)
    {
        var rows = _documentRepository.RemoveDocument(id);
        if (rows == 0)
        {
            throw new ArgumentException("Document with this id does not exist");
        }
        
        foreach (var i in _index)
        {
            i.Remove(id);
        }
    }

    public Database.Model.Document GetDocument(int id)
    {
        var document = _documentRepository.GetDocumentById(id);
        if (document == null)
        {
            throw new ArgumentException("Document with this id does not exist");
        }
        
        return document;
    }
    
    public List<Database.Model.Document> GetAllDocuments()
    {
        return _documentRepository.GetAllDocuments();
    }
}
