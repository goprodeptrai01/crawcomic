namespace ReadMangaTest.Models;

public class Rating
{
    public int Id { get; set; }
    public float Value { get; set; }
    public Comic Comic { get; set; }
}