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
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<ComicGenre> ComicGenres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComicGenre>()
            .HasKey(cg => new { cg.ComicId, cg.CategoryId });
        modelBuilder.Entity<ComicGenre>()
            .HasOne(cg => cg.Comic)
            .WithMany(c => c.ComicCategories)
            .HasForeignKey(cg => cg.ComicId);
        modelBuilder.Entity<ComicGenre>()
            .HasOne(cg => cg.Genre)
            .WithMany(g => g.comicCategories)
            .HasForeignKey(cg => cg.CategoryId);
    }
}