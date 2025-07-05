using CsvHelper.Configuration;
using NoobSoft.PublicLibrary.Database.DataManagement;
using NoobSoft.PublicLibrary.Database.Model;
using Xunit.Abstractions;

namespace NoobSoft.PublicLibrary.Database.Tests;

public class CsvImportTests
{
    private readonly ITestOutputHelper _out;
    
    public CsvImportTests(ITestOutputHelper output)
    {
        _out = output;
    }

    [Fact]
    public void Test_LoadAuthorsFromCsv()
    {
        var authors = LoadTestCsv<Author, AuthorMap>("authors.csv");
        
        Assert.NotEmpty(authors);
        Assert.All(authors, author =>
        {
            Assert.NotEqual(Guid.Empty, author.Id);
            Assert.False(string.IsNullOrWhiteSpace(author.Name));
            Assert.True(author.Birthday.Year > 1800);
        });

        _out.WriteLine($"Loaded {authors.Count} authors");
        _out.WriteLine($"Last: {authors[499].Name}, {authors[499].Birthday:yyyy-MM-dd}");
    }
    
    [Fact]
    public void Test_LoadBooksFromCsv()
    {
        var books = LoadTestCsv<Book,BookMap>("books.csv");
        var authors = LoadTestCsv<Author, AuthorMap>("authors.csv");
        
        var authorsById = authors.ToDictionary(a => a.Id);  // <- link Book.AuthorId to Author.Id in memory (e.g., via Dictionary<Guid, Author>).
        foreach (var book in books)
        {
            book.Author = authorsById[book.AuthorId];   // <- After linking, you set:
        }
        
        Assert.NotEmpty(books);
        Assert.All(books, book => Assert.NotNull(book.Author));
        
        _out.WriteLine($"Loaded {books.Count} books");
        
        var firstBook = books[0];
        _out.WriteLine($"First book: {firstBook.Title}, by: {firstBook.Author.Name}");
    }

    [Fact]
    public void Test_LoadLoanersFromCsv()
    {
        var loaners = LoadTestCsv<Loaner, LoanerMap>("loaners.csv");
        
        Assert.NotEmpty(loaners);
        Assert.All(loaners , loaner =>
        {
            Assert.NotEqual(Guid.Empty, loaner.Id);
            Assert.False(string.IsNullOrWhiteSpace(loaner.Name));
            Assert.True(loaner.Birthday.Year > 1800);
        });
        
        _out.WriteLine($"Loaded {loaners.Count} loaners");
        _out.WriteLine($"First loaner: {loaners[0].Name}, born on {loaners[0].Birthday:yyyy-MM-dd}");
    }
    
    /// <summary>
    /// Generic helper to load a list of records from a CSV file using CsvDataImporter.
    /// </summary>
    private static List<T> LoadTestCsv<T, TMap>(string fileName)
        where TMap : ClassMap<T>, new()
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        return CsvDataImporter.LoadCsv<T, TMap>(fullPath);
    }

}