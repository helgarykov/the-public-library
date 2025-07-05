using NoobSoft.PublicLibrary.Database.DataManagement;
using NoobSoft.PublicLibrary.Database.Model;
using Xunit.Abstractions;

namespace NoobSoft.PublicLibrary.Database.Tests;

public class CsvImportTests
{
    private readonly ITestOutputHelper _out;
    private readonly string _dataFolder;
    
    public CsvImportTests(ITestOutputHelper output)
    {
        _out = output;
        _dataFolder = Path.Combine(AppContext.BaseDirectory, "Data");
    }

    [Fact]
    public void Test_LoadAuthorsFromCsv()
    {
        var path = Path.Combine(_dataFolder, "authors.csv");
        var authors = CsvDataImporter.Load<Author, AuthorMap>(path);


        Assert.NotNull(authors);
        Assert.NotEmpty(authors);

        _out.WriteLine($"Loaded {authors.Count} authors");
        _out.WriteLine($"First: {authors[0].Name}, {authors[0].Birthday:yyyy-MM-dd}");
    }
    
    [Fact]
    public void Test_LoadBooksFromCsv()
    {
        var path = Path.Combine(_dataFolder, "books.csv");
        var books = CsvDataImporter.Load<Book, BookMap>(path);
        
        var authorPath = Path.Combine(_dataFolder, "authors.csv");
        var authors = CsvDataImporter.Load<Author, AuthorMap>(authorPath);
        
        var authorsById = authors.ToDictionary(a => a.Id);  // <- link Book.AuthorId to Author.Id in memory (e.g., via Dictionary<Guid, Author>).
        foreach (var book in books)
        {
            book.Author = authorsById[book.AuthorId];   // <- After linking, you set:
        }

        Assert.NotNull(books);
        Assert.NotEmpty(books);
        
        _out.WriteLine($"Loaded {books.Count} books");
        
        var firstBook = books[0];
        _out.WriteLine($"First book: {firstBook.Title}, by: {firstBook.Author.Name}");
    }
}