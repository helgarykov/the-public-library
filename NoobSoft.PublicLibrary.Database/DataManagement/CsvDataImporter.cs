using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using NoobSoft.PublicLibrary.Database.Model;

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
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item>
    /// <description><c>Records</c>: A list of successfully parsed <typeparamref name="T"/> objects.</description>
    /// </item>
    /// <item>
    /// <description><c>Errors</c>: A list of error messages describing records that could not be parsed (e.g., due to invalid data such as ISBN).</description>
    /// </item>
    /// </list>
    /// </returns>
    public static class CsvDataImporter
    {
        public static (List<T> Records, List<string> Errors) LoadCsv<T, TMap>(string filePath)
            where TMap : ClassMap<T>, new()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HeaderValidated = null,
            };
            
            using var reader = new StreamReader(filePath);  // Opens the CSV file using the provided file path
            using var csv = new CsvReader(reader, config);    // Creates a CsvReader from the file. Uses InvariantCulture to standardize parsing (e.g. dates, numbers).
            
            // Register the custom converter for ISBN after CsvReader is created
            csv.Context.TypeConverterCache.AddConverter<Isbn>(new IsbnConverter());
            
            // Registers your mapping class (like AuthorMap) that tells CsvHelper how to map CSV columns to properties
            csv.Context.RegisterClassMap<TMap>();   
            
            var records = new List<T>();
            var errors = new List<string>();

            while (csv.Read())
            {
                try
                {
                    var record = csv.GetRecord<T>();
                    records.Add(record);
                }
                catch (Exception ex)
                {
                    var root = ex.InnerException ?? ex;
                    errors.Add($"Row {csv.Context.Parser.RawRow}: {root.Message}");
                }
            }
            return (records, errors);
        }
    }    
}

