namespace ReadMangaTest.Models;

public class ComicGenre
{
    public int Id { get; set; }
    public int ComicId { get; set; }
    public Comic Comic { get; set; }
    public int CategoryId { get; set; }
    public Genre Genre { get; set; }
}