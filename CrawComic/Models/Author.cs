namespace ReadMangaTest.Models;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public bool IsHidden { get; set; } = false;
    public List<Comic> Comics { get; set; }
}