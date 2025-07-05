using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace NoobSoft.PublicLibrary.Database.DataManagement
{
    /// <summary>
    /// Loads a CSV file and maps its rows into a list of objects of type <typeparamref name="T"/> using CsvHelper.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize each CSV row into (e.g., Author, Book).</typeparam>
    /// <typeparam name="TMap">
    /// The CsvHelper mapping class for <typeparamref name="T"/> (e.g., AuthorMap). 
    /// Must inherit from <see cref="ClassMap{T}"/> and have a public parameterless constructor.
    /// </typeparam>
    /// <param name="filePath">The path to the CSV file to load.</param>
    /// <returns>A list of <typeparamref name="T"/> objects populated from the CSV data.</returns>
    ///     public static List<T> Load<T, TMap>(string filePath)
    ///     where TMap : ClassMap<T>, new()
    public static class CsvDataImporter
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
}

