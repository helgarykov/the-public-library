namespace NoobSoft.PublicLibrary.Database.Model
{
    /// <summary>
    /// Represents an author in the library system. 
    /// Inherits common personal information from <see cref="Person"/>, 
    /// and adds a collection of books authored by the individual.
    /// </summary>
    public class Author : Person
    {
        public Guid Id { get; set; }
    
        public List<Book> Books { get; set; } = new();
    }
}