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
        private readonly LibraryRepository _repo;
        private readonly List<Author> _authors;
        private readonly List<Book> _books;
        private readonly List<Loaner> _loaners;

        public LibraryRepositoryTests(ITestOutputHelper output)
        {
            _out = output;
            _repo = new LibraryRepository();
            _authors = _repo.GetAllAuthors();
            _books = _repo.GetAllBooks();
            _loaners = _repo.GetAllLoaners();
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
                ISBN = "999-9999999999",
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

