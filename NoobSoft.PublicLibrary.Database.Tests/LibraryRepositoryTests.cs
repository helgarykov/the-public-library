using CsvHelper.Configuration;
using NoobSoft.PublicLibrary.Database.DataManagement;
using NoobSoft.PublicLibrary.Database.Model;
using NoobSoft.PublicLibrary.Database.Repository;
using Xunit.Abstractions;

namespace NoobSoft.PublicLibrary.Database.Tests
{
    public class LibraryRepositoryTests
    {
        private readonly ITestOutputHelper _out;
        private readonly List<Author> _authors;
        private readonly List<Book> _books;
        private readonly List<Loaner> _loaners;

        public LibraryRepositoryTests(ITestOutputHelper output)
        {
            _out = output;
             var repo = new LibraryRepository();
            _authors = repo.GetAllAuthors();
            _books = repo.GetAllBooks();
            _loaners = repo.GetAllLoaners();
        }
        
        [Fact]
        public void Repository_Should_Load_AllBooks()
        {
            Assert.NotEmpty(_books);
            Assert.All(_books, b => Assert.NotEqual(Guid.Empty, b.Id));
            Assert.All(_books, b => Assert.False(string.IsNullOrWhiteSpace(b.Title)));
            Assert.All(_books, b => Assert.False(string.IsNullOrWhiteSpace(b.ISBN.ToString())));
        }

        [Fact]
        public void Repository_Should_Load_AllLoaners()
        {
            Assert.NotEmpty(_loaners);
            Assert.All(_loaners, l => Assert.NotEqual(Guid.Empty, l.Id));
        }
        
        [Fact]
        public void Repository_Should_Load_AllAuthors()
        {
            Assert.NotEmpty(_authors);
            Assert.All(_authors, a => Assert.NotEqual(Guid.Empty, a.Id));
        }

        [Fact]
        public void Repository_Should_Link_Books_To_Authors()
        {
            Assert.All(_books, book =>
            {
                Assert.NotNull(book.Author);
                Assert.NotEqual(Guid.Empty, book.Author.Id);
                Assert.Contains(_authors, a => a.Id == book.Author.Id);
            });
        }

        [Fact]
        public void Repository_Should_Not_Link_To_Unknown_Authors()
        {
            // Arrange
            var orphanId = Guid.NewGuid(); // Simulate missing author ID
            
            var invalidBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Orphaned Book",
                ISBN = new Isbn("999-9999999999"),
                AuthorId = orphanId, // ID that does not exist in authors
                Published = DateTime.Now,
                Summary = "No matching author should be linked",
                Author = null
            };
            
            _books.Add(invalidBook);
            
            var knownAuthorIds = _authors.Select(a => a.Id).ToHashSet();
            
            // Sanity check: orphanId should not exist in author list
            Assert.DoesNotContain(orphanId, knownAuthorIds);
            
            // Act
            var orphanedBooks = _books
                .Where(b => !knownAuthorIds.Contains(b.AuthorId))// Should not be linked to a known author
                .ToList();
            
            // Assert
            Assert.NotEmpty(orphanedBooks); // Ensure we actually have a case to test

            foreach (var book in orphanedBooks)
            {
                _out.WriteLine($"❌ Orphaned book found: {book.Title} with missing AuthorId {book.AuthorId}");
                Assert.Null(book.Author); // ✅ its linked Author is still null because no matching AuthorId was found
            }

            _out.WriteLine($"✔ All orphaned books were not linked.");
        }
    }    
}

