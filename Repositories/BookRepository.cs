using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repositories;

public class BookRepository(LibraryDbContext dbContext) : IBookRepository
{
    public async Task<(IReadOnlyList<Book> Items, int TotalItems)> GetAllAsync(
        int page,
        int pageSize,
        string? category,
        string sort,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim().ToLower();
            query = query.Where(book => book.Category.ToLower() == normalizedCategory);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        query = ApplySorting(query, sort);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    public Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(book => book.Id == id, cancellationToken);
    }

    public async Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        dbContext.Books.Update(book);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Book book, CancellationToken cancellationToken = default)
    {
        dbContext.Books.Remove(book);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Book> ApplySorting(IQueryable<Book> query, string sort)
    {
        return sort.ToLowerInvariant() switch
        {
            "title" => query.OrderBy(book => book.Title).ThenBy(book => book.Id),
            "-title" => query.OrderByDescending(book => book.Title).ThenBy(book => book.Id),
            "author" => query.OrderBy(book => book.Author).ThenBy(book => book.Id),
            "-author" => query.OrderByDescending(book => book.Author).ThenBy(book => book.Id),
            "category" => query.OrderBy(book => book.Category).ThenBy(book => book.Id),
            "-category" => query.OrderByDescending(book => book.Category).ThenBy(book => book.Id),
            "publishedyear" => query.OrderBy(book => book.PublishedYear).ThenBy(book => book.Id),
            "-publishedyear" => query.OrderByDescending(book => book.PublishedYear).ThenBy(book => book.Id),
            "-id" => query.OrderByDescending(book => book.Id),
            _ => query.OrderBy(book => book.Id)
        };
    }
}
