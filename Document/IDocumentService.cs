namespace datastructures_project.Document;
using datastructures_project.Database.Model;

public interface IDocumentService
{
    void AddDocument(string title, string url, string description);
    void RemoveDocument(int id);
    Document GetDocument(int id);
}