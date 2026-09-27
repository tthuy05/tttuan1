using LibraryApi.DTOs;

namespace LibraryApi.Services;

public interface IBookService
{
    Task<PagedResult<BookResponseDto>> GetAllAsync(
        BookQueryParameters query,
        CancellationToken cancellationToken = default);

    Task<BookResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BookResponseDto> CreateAsync(
        BookCreateDto request,
        CancellationToken cancellationToken = default);

    Task<BookResponseDto?> UpdateAsync(
        int id,
        BookUpdateDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
