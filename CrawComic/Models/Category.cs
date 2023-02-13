using System.ComponentModel.DataAnnotations;

namespace ReadMangaTest.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public bool IsHidden { get; set; } = false;
    public ICollection<ComicCategory> comicCategories { get; set; }
}