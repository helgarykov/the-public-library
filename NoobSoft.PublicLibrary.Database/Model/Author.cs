namespace NoobSoft.PublicLibrary.Database.Model;

public class Author : Person
{
    private List<Book> loanedBooks { get; set; }

    public Author(Guid id, string name, DateTime birthday) : base(id, name, birthday)
    {
        loanedBooks = new List<Book>();
    }
}