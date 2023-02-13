namespace ReadMangaTest.Models;

public class Page
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public Chapter Chapter { get; set; }
    public bool IsHidden { get; set; }
}