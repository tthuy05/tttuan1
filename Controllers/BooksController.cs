using LibraryApi.DTOs;
using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/v1/books")]
public class BooksController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BookResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<BookResponseDto>>> GetAll(
        [FromQuery] BookQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await bookService.GetAllAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var book = await bookService.GetByIdAsync(id, cancellationToken);
        return book is null ? BookNotFound(id) : Ok(book);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookResponseDto>> Create(
        BookCreateDto request,
        CancellationToken cancellationToken)
    {
        var createdBook = await bookService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, createdBook);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookResponseDto>> Update(
        int id,
        BookUpdateDto request,
        CancellationToken cancellationToken)
    {
        var updatedBook = await bookService.UpdateAsync(id, request, cancellationToken);
        return updatedBook is null ? BookNotFound(id) : Ok(updatedBook);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await bookService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : BookNotFound(id);
    }

    private NotFoundObjectResult BookNotFound(int id)
    {
        return NotFound(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Book not found",
            Detail = $"No book with id {id} was found."
        });
    }
}
