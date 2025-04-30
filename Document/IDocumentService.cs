namespace datastructures_project.Document;

public interface IDocumentService
{
    void AddDocument(int id, Document document);
    void RemoveDocument(int id);
    Document GetDocument(int id);
}