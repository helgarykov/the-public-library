namespace NoobSoft.PublicLibrary.Database.Model;

public class Loaner : Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
    public List<Book> LoanedBooks { get; set; } = new();
}