using System.Net;
using System.Net.Http.Json;
using LibraryApi.Models;

namespace LibraryApi.Tests;

public sealed class BooksApiTests
{
    [Fact]
    public async Task GetBooks_FiltersSortsAndPaginates()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        await factory.ResetDatabaseAsync(CreateBooks());

        using var firstPageResponse = await client.GetAsync(
            "/api/v1/books?page=1&pageSize=2&category=IT&sort=title");

        Assert.Equal(HttpStatusCode.OK, firstPageResponse.StatusCode);
        var firstPage = await ReadJsonAsync<PagedBooksResponse>(firstPageResponse);
        Assert.Equal(1, firstPage.Page);
        Assert.Equal(2, firstPage.PageSize);
        Assert.Equal(3, firstPage.TotalItems);
        Assert.Equal(2, firstPage.TotalPages);
        Assert.Collection(
            firstPage.Items,
            book => Assert.Equal("Clean Code", book.Title),
            book => Assert.Equal("Domain-Driven Design", book.Title));
        Assert.All(firstPage.Items, book => Assert.Equal("IT", book.Category));

        using var secondPageResponse = await client.GetAsync(
            "/api/v1/books?page=2&pageSize=2&category=IT&sort=title");

        Assert.Equal(HttpStatusCode.OK, secondPageResponse.StatusCode);
        var secondPage = await ReadJsonAsync<PagedBooksResponse>(secondPageResponse);
        var remainingBook = Assert.Single(secondPage.Items);
        Assert.Equal("The Pragmatic Programmer", remainingBook.Title);
    }

    [Fact]
    public async Task BookEndpoints_SupportFullCrudFlow()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        await factory.ResetDatabaseAsync();

        using var createResponse = await client.PostAsJsonAsync("/api/v1/books", new
        {
            title = "Refactoring",
            author = "Martin Fowler",
            category = "IT",
            publishedYear = 1999
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdBook = await ReadJsonAsync<BookResponse>(createResponse);
        Assert.True(createdBook.Id > 0);
        Assert.Equal("Refactoring", createdBook.Title);

        using var getResponse = await client.GetAsync($"/api/v1/books/{createdBook.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetchedBook = await ReadJsonAsync<BookResponse>(getResponse);
        Assert.Equal(createdBook, fetchedBook);

        using var updateResponse = await client.PatchAsJsonAsync(
            $"/api/v1/books/{createdBook.Id}",
            new
            {
                title = "Refactoring, Second Edition",
                publishedYear = 2018
            });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedBook = await ReadJsonAsync<BookResponse>(updateResponse);
        Assert.Equal("Refactoring, Second Edition", updatedBook.Title);
        Assert.Equal(2018, updatedBook.PublishedYear);
        Assert.Equal("Martin Fowler", updatedBook.Author);
        Assert.Equal("IT", updatedBook.Category);

        using var deleteResponse = await client.DeleteAsync($"/api/v1/books/{createdBook.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        using var getDeletedResponse = await client.GetAsync($"/api/v1/books/{createdBook.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    [Fact]
    public async Task CreateBook_WithInvalidBody_ReturnsBadRequest()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        await factory.ResetDatabaseAsync();

        using var response = await client.PostAsJsonAsync("/api/v1/books", new
        {
            title = string.Empty,
            author = "A valid author",
            category = "IT",
            publishedYear = 2024
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("PATCH")]
    [InlineData("DELETE")]
    public async Task BookById_WhenBookDoesNotExist_ReturnsNotFound(string method)
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        await factory.ResetDatabaseAsync();

        using var request = new HttpRequestMessage(
            new HttpMethod(method),
            "/api/v1/books/999999");

        if (method == "PATCH")
        {
            request.Content = JsonContent.Create(new { title = "Missing book" });
        }

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static Book[] CreateBooks() =>
    [
        new Book
        {
            Title = "The Pragmatic Programmer",
            Author = "Andrew Hunt and David Thomas",
            Category = "IT",
            PublishedYear = 1999
        },
        new Book
        {
            Title = "Pride and Prejudice",
            Author = "Jane Austen",
            Category = "Novel",
            PublishedYear = 1813
        },
        new Book
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Category = "IT",
            PublishedYear = 2008
        },
        new Book
        {
            Title = "Sapiens",
            Author = "Yuval Noah Harari",
            Category = "History",
            PublishedYear = 2011
        },
        new Book
        {
            Title = "Domain-Driven Design",
            Author = "Eric Evans",
            Category = "IT",
            PublishedYear = 2003
        }
    ];

    private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<T>();
        return body ?? throw new Xunit.Sdk.XunitException("The response body was empty.");
    }

    private sealed record PagedBooksResponse(
        IReadOnlyList<BookResponse> Items,
        int Page,
        int PageSize,
        int TotalItems,
        int TotalPages);

    private sealed record BookResponse(
        int Id,
        string Title,
        string Author,
        string Category,
        int PublishedYear);
}
