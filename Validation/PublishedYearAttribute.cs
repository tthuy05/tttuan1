using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PublishedYearAttribute : ValidationAttribute
{
    private const int MinimumYear = 1000;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var maximumYear = DateTime.UtcNow.Year;
        return value is int year && year >= MinimumYear && year <= maximumYear
            ? ValidationResult.Success
            : new ValidationResult($"PublishedYear must be between {MinimumYear} and {maximumYear}.");
    }
}
