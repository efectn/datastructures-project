using datastructures_project.Search.Index;
using datastructures_project.Search.Tokenizer;

namespace datastructures_project.Document;

public class DocumentService : IDocumentService
{
    private Dictionary<int, Document> documents; // TODO: use EF Core instead
    private readonly IIndex _index;
    private readonly ITokenizer _tokenizer;

    public DocumentService(IIndex index, ITokenizer tokenizer)
    {
        _index = index;
        _tokenizer = tokenizer;
        documents = new Dictionary<int, Document>();
    }

    public void AddDocument(int id, Document document)
    {
        if (documents.ContainsKey(id))
        {
            throw new ArgumentException("Document with this id already exists");
        }

        documents.Add(id, document);
        _index.Add(id, _tokenizer.Tokenize(document.Title).ToArray());
    }

    public void RemoveDocument(int id)
    {
        if (!documents.ContainsKey(id))
        {
            throw new ArgumentException("Document with this id does not exist");
        }

        documents.Remove(id);
        // TODO: Implement removal from index
    }

    public Document GetDocument(int id)
    {
        return documents[id];
    }
}
