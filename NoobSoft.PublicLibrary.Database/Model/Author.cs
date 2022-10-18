namespace NoobSoft.PublicLibrary.Database.Model;

public class Author : Person
{
    public Guid id { get; }
    private string name { get; }
    private DateTime birthday { get; }
    private List<Book> myBooks { get; }
    public object Id { get; }
    public object Name { get; }
    public object Birthday { get; }

    public Author(Guid id, string name, DateTime birthday) : base(id, name, birthday)
    {
        this.id = id;
        this.name = name;
        this.birthday = birthday;
    }
}