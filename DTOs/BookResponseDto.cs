namespace LibraryApi.DTOs;

public class BookResponseDto
{
    public int Id { get; init; }

    public required string Title { get; init; }

    public required string Author { get; init; }

    public required string Category { get; init; }

    public int PublishedYear { get; init; }
}
