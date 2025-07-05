namespace NoobSoft.PublicLibrary.Database.Model
{
    /// <summary>
    /// Represents a book in the library system, including its identifier, title, 
    /// author, publication date, ISBN, and a summary description.
    /// </summary>
    public class Book
    {
        public Guid Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public Guid AuthorId { get; set; }  // ← map this from CSV (Holds the foreign key (GUID) from books.csv). Is used During CSV import and lookups.
        public Author Author { get; set; }  // ← assign later in code. Holds the full object with Name, Birthday, etc. For use in business logic, tests, display.
        public DateTime Published { get; set; }
        public string Summary { get; set; }
    }
}

