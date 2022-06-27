namespace NoobSoft.PublicLibrary.Database.Model;

public class Book
{
    public Guid Id { get; set; }
    public ISBN ISBN { get; set; }
    public string Title { get; set; }
    public Author Author { get; set; }
    public DateTime Published { get; set; }
    public string Summary { get; set; }
}