namespace NoobSoft.PublicLibrary.Database.Model;

public class Author : Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
    public List<Book> Books { get; set; } = new();
}
