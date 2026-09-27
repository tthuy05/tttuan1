using System.ComponentModel.DataAnnotations;
using LibraryApi.Validation;

namespace LibraryApi.DTOs;

public class BookUpdateDto : IValidatableObject
{
    [StringLength(200)]
    [NotWhiteSpace(ErrorMessage = "Title must not be blank.")]
    public string? Title { get; init; }

    [StringLength(150)]
    [NotWhiteSpace(ErrorMessage = "Author must not be blank.")]
    public string? Author { get; init; }

    [StringLength(100)]
    [NotWhiteSpace(ErrorMessage = "Category must not be blank.")]
    public string? Category { get; init; }

    [PublishedYear]
    public int? PublishedYear { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Title is null && Author is null && Category is null && PublishedYear is null)
        {
            yield return new ValidationResult(
                "At least one field must be provided.",
                [nameof(Title), nameof(Author), nameof(Category), nameof(PublishedYear)]);
        }
    }
}
