using CsvHelper.Configuration;
using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.DataManagement
{
    /// <summary>
    /// Maps Author properties to CSV columns.
    /// </summary>
    
    public sealed class AuthorMap : ClassMap<Author>
    {
        public AuthorMap()
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.Name).Name("Name");
            Map(m => m.Birthday)
                .Name("Birthday")
                .TypeConverterOption
                .Format("MM/dd/yyyy HH:mm:ss");

        }
    }
}

