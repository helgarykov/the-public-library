using CsvHelper.Configuration;
using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.DataManagement
{
    /// <summary>
    /// Maps Book properties to CSV columns.
    /// </summary>
    public sealed class BookMap : ClassMap<Book>
    {
        public BookMap()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.ISBN).Name("ISBN");
            Map(m => m.Title).Name("Title");
            Map(m => m.AuthorId).Name("Author");    // ← this matches the author 
            Map(m => m.Published)
                .Name("Published")
                .TypeConverterOption
                .Format("MM/dd/yyyy HH:mm:ss");
            Map(m => m.Summary).Name("Summary");
        }
    
    }
}

