using LibraryApi.DTOs;
using LibraryApi.Models;
using LibraryApi.Repositories;

namespace LibraryApi.Services;

public class BookService(IBookRepository bookRepository) : IBookService
{
    public async Task<PagedResult<BookResponseDto>> GetAllAsync(
        BookQueryParameters query,
        CancellationToken cancellationToken = default)
    {
        var (items, totalItems) = await bookRepository.GetAllAsync(
            query.Page,
            query.PageSize,
            query.Category,
            query.Sort,
            cancellationToken);

        return new PagedResult<BookResponseDto>
        {
            Items = items.Select(ToResponse).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<BookResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(id, cancellationToken);
        return book is null ? null : ToResponse(book);
    }

    public async Task<BookResponseDto> CreateAsync(
        BookCreateDto request,
        CancellationToken cancellationToken = default)
    {
        var book = new Book
        {
            Title = request.Title.Trim(),
            Author = request.Author.Trim(),
            Category = request.Category.Trim(),
            PublishedYear = request.PublishedYear
        };

        await bookRepository.AddAsync(book, cancellationToken);
        return ToResponse(book);
    }

    public async Task<BookResponseDto?> UpdateAsync(
        int id,
        BookUpdateDto request,
        CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
        {
            return null;
        }

        if (request.Title is not null)
        {
            book.Title = request.Title.Trim();
        }

        if (request.Author is not null)
        {
            book.Author = request.Author.Trim();
        }

        if (request.Category is not null)
        {
            book.Category = request.Category.Trim();
        }

        if (request.PublishedYear.HasValue)
        {
            book.PublishedYear = request.PublishedYear.Value;
        }

        await bookRepository.UpdateAsync(book, cancellationToken);
        return ToResponse(book);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
        {
            return false;
        }

        await bookRepository.DeleteAsync(book, cancellationToken);
        return true;
    }

    private static BookResponseDto ToResponse(Book book)
    {
        return new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            PublishedYear = book.PublishedYear
        };
    }
}
