using NoobSoft.PublicLibrary.Database.Model;
using Xunit.Abstractions;

namespace NoobSoft.PublicLibrary.Database.Tests;

public class DataLoaderTests
{
    private readonly ITestOutputHelper _out;

    public DataLoaderTests(ITestOutputHelper @out)
    {
        _out = @out;
    }
    
    [Fact]
    public void TestAuthorAndBooks()
    {
        var author = new Author()
        {
            Id = Guid.NewGuid(),
            Birthday = new DateTime(1899, 7, 21),
            Name = "Ernest Hemingway",
            Books = new List<Book>(),
        };

        var b1 = new Book()
        {
            Id = Guid.NewGuid(),
            Title = "A Farewell to Arms",
            ISBN = new ISBN("978-1124000138"),
            Author = author,
            Published = new DateTime(1929, 1, 1),
            Summary = "The unforgettable story of an American " +
                      "ambulance driver on the Italian front " +
                      "and his passion for a beautiful English " +
                      "nurse."
        };
        
        author.Books.Add(b1);

        var b2 = new Book()
        {
            Id = Guid.NewGuid(),
            Title = "For Whom The Bell Tolls",
            ISBN = new ISBN("978-1476787817"),
            Author = author,
            Published = new DateTime(1940, 1, 1),
            Summary = "Robert Jordan is a young American in the " +
                      "International Brigades attached to an " +
                      "antifascist guerilla unit in the mountains " +
                      "of Spain. In his portrayal of Jordan’s love " +
                      "for the beautiful Maria and his superb " +
                      "account of El Sordo’s last stand, Hemingway " +
                      "creates a work at once rare and beautiful, " +
                      "strong and brutal, compassionate, moving, " +
                      "and wise."
        };
        
        author.Books.Add(b2);
        
        Assert.Equal(2, author.Books.Count);
        Assert.Equal("978-1476787817", b2.ISBN.ToString());
    }
}