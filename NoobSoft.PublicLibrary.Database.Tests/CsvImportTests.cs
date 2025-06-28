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
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "authors.csv");
        var authors = CsvDataImporter.Load<Author, AuthorMap>(path);


        Assert.NotNull(authors);
        Assert.NotEmpty(authors);

        _out.WriteLine($"Loaded {authors.Count} authors");
        _out.WriteLine($"First: {authors.First().Name}, {authors.First().Birthday:yyyy-MM-dd}");
    }
}