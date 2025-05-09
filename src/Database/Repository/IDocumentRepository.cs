namespace datastructures_project.Database.Repository;

public interface IDocumentRepository
{
    (Model.Document, int) CreateDocument(string title, string url, string description);
    Model.Document GetDocumentById(int id);
    List<Model.Document> GetAllDocuments();
    int RemoveDocument(int id);
    int AllDocumentsCount();
}