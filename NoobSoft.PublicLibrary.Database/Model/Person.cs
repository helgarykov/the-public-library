using CsvHelper.Configuration.Attributes;

namespace NoobSoft.PublicLibrary.Database.Model;

public abstract class Person
{
    [Index(0)]
    private Guid id { get; }
    [Index(1)]
    private string name { get; }
    [Index(2)]
    private DateTime birthday { get; }
    
    protected Person(Guid id, string name, DateTime birthday)
    {
        this.id = id;
        this.name = name;
        this.birthday = birthday;
    }
}