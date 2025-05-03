using datastructures_project.Database;
using datastructures_project.Database.Model;
using datastructures_project.Database.Repository;
using Microsoft.EntityFrameworkCore;

namespace tests.Search.Database.Repository;

public class DocumentRepositoryTest
{
    public class DocumentRepositoryTests
    {
        private ApplicationDbContext _getInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // to have different db for each test
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void DocumentRepository_CreateDocument()
        {
            var context = _getInMemoryDbContext();
            var repository = new DocumentRepository(context);
            var (doc, rows) = repository.CreateDocument("Test Title", "https://example.com", "Test Description");

            Assert.Equal(1, rows);
            Assert.NotEqual(0, doc.Id);
            Assert.Equal("Test Title", doc.Title);
        }

        [Fact]
        public void DocumentRepository_GetDocumentById()
        {
            var context = _getInMemoryDbContext();
            var doc = new datastructures_project.Database.Model.Document("Sample", "https://sample.com", "Sample Desc");
            context.Documents.Add(doc);
            context.SaveChanges();

            var repository = new DocumentRepository(context);
            var result = repository.GetDocumentById(doc.Id);

            Assert.NotNull(result);
            Assert.Equal("Sample", result.Title);
        }

        [Fact]
        public void DocumentRepository_GetAllDocuments()
        {
            var context = _getInMemoryDbContext();
            context.Documents.AddRange(
                new datastructures_project.Database.Model.Document("Doc1", "http://url1.com", "desc1"),
                new datastructures_project.Database.Model.Document("Doc2", "http://url2.com", "desc2")
            );
            context.SaveChanges();

            var repository = new DocumentRepository(context);
            var result = repository.GetAllDocuments();
            Assert.Equal(12, result.Count); // 10 comes from the seeder
        }

        [Fact]
        public void DocumentRepository_RemoveDocument()
        {
            var context = _getInMemoryDbContext();
            var doc = new datastructures_project.Database.Model.Document("ToDelete", "http://url.com", "desc");
            context.Documents.Add(doc);
            context.SaveChanges();

            var repository = new DocumentRepository(context);
            var rows = repository.RemoveDocument(doc.Id);

            Assert.Equal(1, rows);
            Assert.Null(context.Documents.Find(doc.Id));
        }

        [Fact]
        public void DocumentRepository_RemoveDocument_WithInvalidId()
        {
            var context = _getInMemoryDbContext();
            var repository = new DocumentRepository(context);

            var result = repository.RemoveDocument(999); // non-existent id

            Assert.Equal(0, result);
        }

        [Fact]
        public void DocumentRepository_CheckWhetherDocumentSeeded()
        {
            var context = _getInMemoryDbContext();
            var repository = new DocumentRepository(context);

            var documents = repository.GetAllDocuments();

            Assert.NotEmpty(documents);
            Assert.Equal(10, documents.Count); // Check if 10 seeded documents exist
        }
    }
}