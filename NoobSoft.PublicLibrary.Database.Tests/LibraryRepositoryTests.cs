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
        private readonly List<string> _importLog; 

        public LibraryRepositoryTests(ITestOutputHelper output)
        {
            _out = output;

            var testDataPath = Path.Combine(AppContext.BaseDirectory, "Data");
            var importer = new CsvDataImporterService(testDataPath);
            
             _repo = new LibraryRepository(importer);
             _repo.LoadData();   // explicit loading instead of constructor logic
             
            _importLog = _repo.ImportLog;
            _authors = _repo.GetAllAuthors();
            _books = _repo.GetAllBooks();
            _loaners = _repo.GetAllLoaners();
            
            foreach (var error in _importLog)
            {
                _out.WriteLine($"⚠️ Import error: {error}");
            }
        }
        
        [Fact]
        public void Should_Log_Exactly_Three_Invalid_ISBNs()
        {
            var invalidIsbnErrors = _importLog.Where(e => e.Contains("Invalid ISBN")).ToList();
            Assert.Equal(3, invalidIsbnErrors.Count);
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
                ISBN = new Isbn("9780306406157"),   // ← known valid ISBN-13
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
        

        [Fact]
        public void FindBooksByPartialIsbn_Should_Find_Matching_Books()
        {
            string partialIsbn = "97841";

            var result = _repo.FindBooksByPartialIsbn(partialIsbn);

            Assert.NotEmpty(result);
            Assert.All(result, book => Assert.Contains(partialIsbn, book.ISBN.ToString()));

            int count = 1;
            foreach (var book in result)
            {
                _out.WriteLine($"Found book {count ++}: {book.Title} with ISBN: {book.ISBN}");
            }
        }

        [Fact]
        public void FindPeopleByPartialName_Should_Find_Matching_Authors_Or_Loaners()
        {
            string partialName = "Bi";
            string birthday = "01/06/1885";
            
            // Act
            var resultByName = _repo.FindPeopleByPartialNameOrBirthday(partialName);
            var resultByDate = _repo.FindPeopleByPartialNameOrBirthday(birthday);

            var combinedResults = resultByName.Concat(resultByDate).Distinct().ToList();
            
            int count = 1;
            foreach (var person in combinedResults)
            {
                Assert.NotNull(person.Name);
                var type = person is Author ? "Author" :
                    person is Loaner ? "Loaner" : "Unknown";

                _out.WriteLine($"👤 Person {count++}: [{type}] '{person.Name}', Birthday: {person.Birthday:yyyy-MM-dd}");
            }
        }
        
    }    
}

