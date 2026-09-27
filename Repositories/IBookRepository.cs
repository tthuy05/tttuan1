using LibraryApi.Models;

namespace LibraryApi.Repositories;

public interface IBookRepository
{
    Task<(IReadOnlyList<Book> Items, int TotalItems)> GetAllAsync(
        int page,
        int pageSize,
        string? category,
        string sort,
        CancellationToken cancellationToken = default);

    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default);

    Task UpdateAsync(Book book, CancellationToken cancellationToken = default);

    Task DeleteAsync(Book book, CancellationToken cancellationToken = default);
}
