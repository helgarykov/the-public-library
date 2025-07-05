namespace NoobSoft.PublicLibrary.Database.Model
{
    /// <summary>
    /// Represents a book in the library system, including its identifier, title, 
    /// author, publication date, ISBN, and a summary description.
    /// </summary>
    public class Book
    {
        public Guid Id { get; set; }
        public ISBN ISBN { get; set; }
        public string Title { get; set; }
        public Author Author { get; set; }
        public DateTime Published { get; set; }
        public string Summary { get; set; }
    }
}

