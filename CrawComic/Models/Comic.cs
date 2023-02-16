using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadMangaTest.Models;

public class Comic
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AlternateName { get; set; }
    public string Description { get; set; }
    public string Wallpaper { get; set; }
    public string Url { get; set; }
    public List<Author> Authors { get; set; }
    public List<Artist> Artist { get; set; }
    public virtual List<Chapter> Chapters { get; set; }
    public Rating Rating { get; set; }
    public List<ComicGenre> ComicCategories { get; set; }
    public bool IsHidden { get; set; } = false;

}