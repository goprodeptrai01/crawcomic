using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadMangaTest.Models;

public class Comic
{

    public string Name { get; set; }
    public string Description { get; set; }
    public string Wallpaper { get; set; }
    public string Url { get; set; }
    public Artist Artist { get; set; }
    public bool IsHidden { get; set; } = false;
    public virtual List<Chapter> Chapters { get; set; }

    public List<ComicCategory> ComicCategories { get; set; }
}