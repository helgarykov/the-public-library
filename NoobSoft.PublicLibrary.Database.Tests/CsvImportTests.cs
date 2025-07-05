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
}