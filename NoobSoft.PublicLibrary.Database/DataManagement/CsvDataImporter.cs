using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace NoobSoft.PublicLibrary.Database.DataManagement;

public class CsvDataImporter
{
    public static List<T> Load<T, TMap>(string filePath)
        where TMap : ClassMap<T>, new()
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<TMap>();
        return csv.GetRecords<T>().ToList();
    }
}