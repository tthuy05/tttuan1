using System.ComponentModel.DataAnnotations;
using LibraryApi.Validation;

namespace LibraryApi.DTOs;

public class BookQueryParameters : IValidatableObject
{
    [Range(1, 1_000_000)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 10;

    [StringLength(100)]
    [NotWhiteSpace(ErrorMessage = "Category must not be blank when provided.")]
    public string? Category { get; init; }

    public string Sort { get; init; } = "id";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        string[] allowedSortValues =
        [
            "id", "title", "author", "category", "publishedYear",
            "-id", "-title", "-author", "-category", "-publishedYear"
        ];

        if (!allowedSortValues.Contains(Sort, StringComparer.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                "Sort must be one of: id, title, author, category, publishedYear. Prefix with '-' for descending order.",
                [nameof(Sort)]);
        }
    }
}
