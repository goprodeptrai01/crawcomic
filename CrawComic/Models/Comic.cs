using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadMangaTest.Models;

public class Comic
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string Wallpaper { get; set; }
    public string Url { get; set; }
    public Artist Artist { get; set; }
    public bool IsHidden { get; set; } = false;
    public virtual ICollection<Chapter> Chapters { get; set; }

    public ICollection<ComicCategory> ComicCategories { get; set; }
}