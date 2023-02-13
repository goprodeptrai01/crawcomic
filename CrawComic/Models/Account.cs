namespace ReadMangaTest.Models;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Role { get; set; }
    public bool IsHidden { get; set; }
}