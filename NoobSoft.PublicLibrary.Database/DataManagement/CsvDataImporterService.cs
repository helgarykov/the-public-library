using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.DataManagement
{

    public class CsvDataImporterService : ICsvDataImporter
    {
        private readonly string _authorFilePath;
        private readonly string _bookFilePath;
        private readonly string _loanerFilePath;

        public CsvDataImporterService(string dataDirectory)
        {
            _authorFilePath = Path.Combine(dataDirectory, "authors.csv");
            _bookFilePath = Path.Combine(dataDirectory, "books.csv");
            _loanerFilePath = Path.Combine(dataDirectory, "loaners.csv");
        }

        public (List<Author> Records, List<string> Errors) ImportAuthors()
            => LoadCsv<Author, AuthorMap>(_authorFilePath);
        
        public (List<Book> Records, List<string> Errors) ImportBooks()
            => LoadCsv<Book, BookMap>(_bookFilePath);
        
        public (List<Loaner> Records, List<string> Errors) ImportLoaners()
            => LoadCsv<Loaner, LoanerMap>(_loanerFilePath);
        
        /// <summary>
        /// Loads and parses a CSV file into a list of typed records using CsvHelper and the specified map.
        /// </summary>
        internal (List<T> Records, List<string> Errors) LoadCsv<T, TMap>(string filePath)
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
                    //Console.WriteLine($"📄 RAW LINE [{csv.Context.Parser.Row}]: {csv.Context.Parser.RawRecord}");

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

