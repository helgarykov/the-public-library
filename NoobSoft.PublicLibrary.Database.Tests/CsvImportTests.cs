using CsvHelper.Configuration;
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
        // var path = Path.Combine(_dataFolder, "authors.csv");
        // var authors = CsvDataImporter.Load<Author, AuthorMap>(path);
        
        var authors = LoadTestCsv<Author, AuthorMap>("authors.csv");
        
        Assert.NotNull(authors);
        Assert.NotEmpty(authors);

        _out.WriteLine($"Loaded {authors.Count} authors");
        _out.WriteLine($"First: {authors[499].Name}, {authors[499].Birthday:yyyy-MM-dd}");
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

        Assert.NotNull(books);
        Assert.NotEmpty(books);
        
        _out.WriteLine($"Loaded {books.Count} books");
        
        var firstBook = books[0];
        _out.WriteLine($"First book: {firstBook.Title}, by: {firstBook.Author.Name}");
    }
    
    /// <summary>
    /// Generic helper to load a list of records from a CSV file using CsvDataImporter.
    /// </summary>
    private List<T> LoadTestCsv<T, TMap>(string fileName)
        where TMap : ClassMap<T>, new()
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        return CsvDataImporter.LoadCsv<T, TMap>(fullPath);
    }

}