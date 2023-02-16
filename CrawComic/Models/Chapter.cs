using System.ComponentModel.DataAnnotations;

namespace ReadMangaTest.Models;

public class Chapter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public bool IsHidden { get; set; } = false;
    public IList<Page> Pages { get; set; }
    public Comic Comic { get; set; }
}