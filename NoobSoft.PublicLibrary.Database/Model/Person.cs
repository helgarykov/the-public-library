using CsvHelper.Configuration.Attributes;

namespace NoobSoft.PublicLibrary.Database.Model;

public abstract class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime Birthday { get; set; }

    protected Person() {} // CsvHelper requires a parameterless constructor

    protected Person(Guid id, string name, DateTime birthday)
    {
        Id = id;
        Name = name;
        Birthday = birthday;
    }
}
