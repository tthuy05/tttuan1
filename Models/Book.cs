namespace LibraryApi.Models;

public class Book
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Author { get; set; }

    public required string Category { get; set; }

    public int PublishedYear { get; set; }
}
