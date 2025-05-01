using Microsoft.EntityFrameworkCore;
using datastructures_project.Database.Model;
using datastructures_project.Search.Index;
using datastructures_project.Search.Tokenizer;

namespace datastructures_project.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Model.Document> Documents { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed the data
        Seed.DocumentSeeder.Seed(modelBuilder);
    }
}
