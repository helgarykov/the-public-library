using CsvHelper.Configuration;
using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.DataManagement
{
    /// <summary>
    /// Maps Loaner properties to CSV columns.
    /// </summary>
    public sealed class LoanerMap : ClassMap<Loaner>
    {
        public LoanerMap()
        {
            Map(l => l.Id).Name("Id");
            Map(l => l.Name).Name("Name");
            Map(l => l.Birthday)
                .Name("Birthday")
                .TypeConverterOption
                .Format("MM/dd/yyyy HH:mm:ss");
        }
    }
}

