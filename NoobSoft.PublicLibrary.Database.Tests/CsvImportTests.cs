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
        var (authors, authorErrors) = LoadTestCsvWithErrors<Author, AuthorMap>("authors.csv");
        
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
        var (books, bookErrors) = LoadTestCsvWithErrors<Book, BookMap>("books.csv");
        var (authors, authorErrors) = LoadTestCsvWithErrors<Author, AuthorMap>("authors.csv");

        // Log any CSV errors
        foreach (var error in bookErrors)
            _out.WriteLine($"❌ Book import error: {error}");
        foreach (var error in authorErrors)
            _out.WriteLine($"❌ Author import error: {error}");
        
        // Link books to authors by AuthorId
        var authorsById = authors.ToDictionary(a => a.Id);  
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
        var (loaners, errors) = LoadTestCsvWithErrors<Loaner, LoanerMap>("loaners.csv");
        
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
    
    [Fact]
    public void Test_BooksCsv_Should_Report_InvalidIsbns()
    {
        var (books, errors) = LoadTestCsvWithErrors<Book, BookMap>("books.csv");

        Assert.NotEmpty(books);
        Assert.Contains(errors, e => e.Contains("Invalid ISBN"));

        _out.WriteLine($"Imported {books.Count} books with {errors.Count} errors.");
        foreach (var err in errors)
            _out.WriteLine($"❌ {err}");
    }

    
    /// <summary>
    /// Loads a CSV file and maps its contents into a list of records of type <typeparamref name="T"/> using CsvHelper.
    /// Also captures any conversion or mapping errors (e.g., invalid data such as malformed ISBNs).
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize each CSV row into (e.g., <c>Book</c>, <c>Author</c>).</typeparam>
    /// <typeparam name="TMap">
    /// The CsvHelper mapping class that defines how CSV columns map to properties of <typeparamref name="T"/>.
    /// Must inherit from <see cref="ClassMap{T}"/> and have a public parameterless constructor.
    /// </typeparam>
    /// <param name="fileName">The file name (within the "Data" folder) of the CSV file to load.</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item>
    /// <description><c>Records</c>: A list of successfully parsed <typeparamref name="T"/> objects.</description>
    /// </item>
    /// <item>
    /// <description><c>Errors</c>: A list of strings describing each error encountered during parsing.</description>
    /// </item>
    /// </list>
    /// </returns>
    private static (List<T> Records, List<string> Errors) LoadTestCsvWithErrors<T, TMap>(string fileName)
        where TMap : ClassMap<T>, new()
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        return CsvDataImporter.LoadCsv<T, TMap>(fullPath);
    }

}