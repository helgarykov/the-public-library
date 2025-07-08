using NoobSoft.PublicLibrary.Database.DataManagement;
using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.Repository
{
    /// <summary>
    /// Provides a centralized data access point for the application, loading and managing
    /// collections of <see cref="Author"/>, <see cref="Book"/>, and <see cref="Loaner"/> entities.
    /// Enables retrieval of all records or a specific item by its <see cref="Guid"/> identifier.
    /// </summary>
    public class LibraryRepository
    {
        private List<Author> _authors;
        private List<Book> _books;
        private List<Loaner> _loaners;

        public List<string> ImportLog { get; } = new(); // ← Store all import errors

        public void LoadData()
        {
            var (authorRecords, authorErrors) = CsvDataImporter.LoadCsv<Author, AuthorMap>("Data/authors.csv");
            var (bookRecords, bookErrors) = CsvDataImporter.LoadCsv<Book, BookMap>("Data/books.csv");
            var (loanerRecords, loanerErrors) = CsvDataImporter.LoadCsv<Loaner, LoanerMap>("Data/loaners.csv");
            
            _authors = authorRecords;
            _books = bookRecords;
            _loaners = loanerRecords;
            
            ImportLog.AddRange(authorErrors);
            ImportLog.AddRange(bookErrors);
            ImportLog.AddRange(loanerErrors);
            
            LinkBooksToAuthors();
        }
        public List<Author> GetAllAuthors() => _authors;
        public List<Book> GetAllBooks() => _books;
        public List<Loaner> GetAllLoaners() => _loaners;
        
        public Author? GetAuthorById(Guid id) => _authors.FirstOrDefault(a => a.Id == id);
        public Book? GetBookById(Guid id) => _books.FirstOrDefault(b => b.Id == id);
        public Loaner? GetLoanerById(Guid id) => _loaners.FirstOrDefault(l => l.Id == id);
        
        
        /// <summary>
        /// Finds and returns a list of books whose ISBN contains the specified partial string.
        /// </summary>
        /// <param name="partialIsbn">The partial ISBN string to search for (case-insensitive).</param>
        /// <returns>
        /// A list of <see cref="Book"/> objects where the ISBN contains the given partial string.
        /// </returns>
        /// <remarks>
        /// This method performs a case-insensitive substring match on the string representation of the ISBN.
        /// Useful for scenarios where the full ISBN is not known or only part of it is available.
        /// </remarks>
        public List<Book> FindBooksByPartialIsbn(string partialIsbn)
        {
            return _books
                .Where(b => b.ISBN.ToString().Contains(partialIsbn, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        
        /// <summary>
        /// Searches for books whose ISBN contains the specified partial string, using a case-insensitive comparison.
        /// </summary>
        /// <param name="partialIsbn">The partial ISBN string to search for.</param>
        /// <returns>
        /// A list of <see cref="Book"/> objects whose ISBNs contain the given partial string.
        /// The list will contain:
        /// <list type="bullet">
        /// <item><description>Multiple books if more than one match is found.</description></item>
        /// <item><description>One book if only a single match is found.</description></item>
        /// <item><description>An empty list if no matches are found.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// The search uses the string representation of the <see cref="Isbn"/> object and performs a case-insensitive substring match.
        /// </remarks>
        public List<Person> FindPeopleByPartioalNameOrBirthday(string search)
        {
            search = search.ToLowerInvariant();
            var isDate = DateTime.TryParse(search, out DateTime parsedDate);

            var matches = new List<Person>();
            
            matches.AddRange(_authors 
                .Where(a => a.Name.ToLowerInvariant().Contains(search) || (isDate && a.Birthday.Date == parsedDate.Date)));
            
            matches.AddRange(_loaners
                .Where(l => l.Name.ToLowerInvariant().Contains(search) || (isDate && l.Birthday.Date == parsedDate)));
            
            return matches;
        }
        
        
        /// <summary>
        /// Resolves and assigns full <see cref="Author"/> objects to each <see cref="Book"/>
        /// based on their corresponding <c>AuthorId</c> values.
        /// This links previously loaded book records to their respective authors.
        /// </summary>
        private void LinkBooksToAuthors()
        {
            var authorsById = _authors.ToDictionary(a => a.Id);
            foreach (var book in _books)
            {
                if (authorsById.TryGetValue(book.AuthorId, out var author))
                {
                    book.Author = author;  // ← only if author exists
                }
                else
                {
                    book.Author = null;  // or leave it unassigned
                }
            }
        }

        /// <summary>
        /// Placeholder for linking loaners to their loaned books.
        /// Not implemented because loaners.csv contains no loan information.
        /// </summary>
        private void LinkLoanersToBooks()
        {
            // No loan data available to perform linking.
        }
    }
}
