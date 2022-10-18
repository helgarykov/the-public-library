namespace NoobSoft.PublicLibrary.Database.Model;

public class Loaner : Person
{
    private Guid Id { get; set; }
    private string Name { get; set; }
    private DateTime Birthday { get; set; }
    private List<Book> loanedBooks { get; set; }

    public Loaner(Guid id, string name, DateTime birthday) : base(id, name, birthday)
    {
    }
}