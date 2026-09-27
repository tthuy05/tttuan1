using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var book = modelBuilder.Entity<Book>();
        book.Property(item => item.Title).IsRequired().HasMaxLength(200);
        book.Property(item => item.Author).IsRequired().HasMaxLength(150);
        book.Property(item => item.Category).IsRequired().HasMaxLength(100);
        book.HasIndex(item => item.Category);

        book.HasData(
            new Book
            {
                Id = 1,
                Title = "Clean Code",
                Author = "Robert C. Martin",
                Category = "IT",
                PublishedYear = 2008
            },
            new Book
            {
                Id = 2,
                Title = "The Pragmatic Programmer",
                Author = "David Thomas & Andrew Hunt",
                Category = "IT",
                PublishedYear = 1999
            },
            new Book
            {
                Id = 3,
                Title = "Design Patterns",
                Author = "Erich Gamma et al.",
                Category = "IT",
                PublishedYear = 1994
            },
            new Book
            {
                Id = 4,
                Title = "Norwegian Wood",
                Author = "Haruki Murakami",
                Category = "Novel",
                PublishedYear = 1987
            },
            new Book
            {
                Id = 5,
                Title = "A Brief History of Time",
                Author = "Stephen Hawking",
                Category = "Science",
                PublishedYear = 1988
            });
    }
}
