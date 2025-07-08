namespace NoobSoft.PublicLibrary.Database.Model;

public class Loaner : Person
{
    public Guid Id { get; set; }
    
    public List<Book> LoanedBooks { get; set; } = new();
    
    public override string ToString() => $"{Name} ({Birthday:yyyy-MM-dd})";

}