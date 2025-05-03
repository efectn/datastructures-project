using Microsoft.EntityFrameworkCore;
using datastructures_project.Database.Model;
using datastructures_project.Search.Index;
using datastructures_project.Search.Tokenizer;

namespace datastructures_project.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Model.Document> Documents { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public ApplicationDbContext() : base()
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Data Source=database.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed the data
        Seed.DocumentSeeder.Seed(modelBuilder);
    }
}
