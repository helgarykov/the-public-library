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
        private readonly List<Author> _authors;
        private readonly List<Book> _books;
        private readonly List<Loaner> _loaners;

        public LibraryRepository()
        {
            _authors = CsvDataImporter.LoadCsv<Author, AuthorMap>("Data/authors.csv");
            _books = CsvDataImporter.LoadCsv<Book, BookMap>("Data/books.csv");
            _loaners = CsvDataImporter.LoadCsv<Loaner, LoanerMap>("Data/loaners.csv");
            
            LinkBooksToAuthors();
        }

        public List<Author> GetAllAuthors() => _authors;
        public List<Book> GetAllBooks() => _books;
        public List<Loaner> GetAllLoaners() => _loaners;
        
        public Author? GetAuthorById(Guid id) => _authors.FirstOrDefault(a => a.Id == id);
        public Book? GetBookById(Guid id) => _books.FirstOrDefault(b => b.Id == id);
        public Loaner? GetLoanerById(Guid id) => _loaners.FirstOrDefault(l => l.Id == id);
    
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
