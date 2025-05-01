namespace datastructures_project.Database.Repository;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _ctx;
    
    public DocumentRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public (Model.Document, int) CreateDocument(string title, string url, string description)
    {
        var document = new Model.Document(title, url, description);
        _ctx.Documents.Add(document);

        var rows = _ctx.SaveChanges();
        
        return (document, rows);
    }
    
    public Model.Document GetDocumentById(int id)
    {
        return _ctx.Documents.Find(id);
    }
    
    public List<Model.Document> GetAllDocuments()
    {
        return _ctx.Documents.ToList();
    }
    
    public int RemoveDocument(int id)
    {
        var document = _ctx.Documents.Find(id);
        if (document == null)
        {
            return 0;
        }
        
        _ctx.Documents.Remove(document);
        return _ctx.SaveChanges();
    }
}