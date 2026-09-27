using System.ComponentModel.DataAnnotations;
using LibraryApi.Validation;

namespace LibraryApi.DTOs;

public class BookCreateDto
{
    [Required]
    [StringLength(200)]
    [NotWhiteSpace(ErrorMessage = "Title must not be blank.")]
    public required string Title { get; init; }

    [Required]
    [StringLength(150)]
    [NotWhiteSpace(ErrorMessage = "Author must not be blank.")]
    public required string Author { get; init; }

    [Required]
    [StringLength(100)]
    [NotWhiteSpace(ErrorMessage = "Category must not be blank.")]
    public required string Category { get; init; }

    [PublishedYear]
    public int PublishedYear { get; init; }
}
