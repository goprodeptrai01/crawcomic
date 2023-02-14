using Microsoft.EntityFrameworkCore;
using ReadMangaTest.Models;

namespace ReadMangaTest.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Comic> Comics { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<ComicCategory> ComicCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComicCategory>()
            .HasKey(cg => new { cg.ComicId, cg.CategoryId });
        modelBuilder.Entity<ComicCategory>()
            .HasOne(cg => cg.Comic)
            .WithMany(c => c.ComicCategories)
            .HasForeignKey(cg => cg.ComicId);
        modelBuilder.Entity<ComicCategory>()
            .HasOne(cg => cg.Category)
            .WithMany(g => g.comicCategories)
            .HasForeignKey(cg => cg.CategoryId);
    }
}