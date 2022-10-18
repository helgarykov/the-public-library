using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.DataManagement;

public sealed class AuthorDataImport : ClassMap<Author>, IDataImportProcessor
{
    /// <summary>
    /// Constructor to create an instance of StreamReader Author-object.
    /// </summary>
    public AuthorDataImport()
    {
        Map(m => m.Id).Name("id");
        Map(m => m.Name).Name("name");
        Map(m => m.Birthday).Name("birthday");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };
        using var authorsFile = new StreamReader(path: "Data/authors.csv");
        using var csv = new CsvReader(authorsFile, config);
        // {
        //     csv.Context.RegisterClassMap<AuthorDataImport>();
        //     var authorRecords = csv.GetRecords<Author>().ToList();
        // }

    }
    
    public string[] GetIdRecords(CsvReader authors)
    {
        csv.Context.RegisterClassMap<AuthorDataImport>();
        var authorRecords = csv.GetRecords<Author>();
        
        //throw new NotImplementedException();
    }

    public Dictionary<string, List<object>> ImportData(string file)
    {
        throw new NotImplementedException();
    }
    
    
   /* private Guid id { get; }
    private string name { get; }
    private DateTime birthday { get; }
    
    public AuthorDataImport(Guid id, string name, DateTime birthday) : base(id, name, birthday)
    {
        this.id = id;
        this.name = name;
        this.birthday = birthday;
    }


    public void GetDatas()
    {
        try
        {
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader authours = new StreamReader("Data/authors.csv"))
            {
                string line;
                // Read and display lines from the file until the end of
                // the file is reached.
                while ((line = authours.ReadLine()) != null)
                {
                    line.Split(',');
                    
                }
            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
        
    }
    public string? GetData(string path)
    {
        var fileAllLines = new StreamReader("Data/authors.csv");
        var readAllLines = fileAllLines.ReadLine();
        return readAllLines;
    }

    public string[] GetAuthorID(string fileLines)
    {

        var importLines = GetData(fileLines);
           // fileLines.Split(',');
        var authorID = importLines[0];
    }

    public Dictionary<string, List<object>> ImportData(string file)
    {
        throw new NotImplementedException();
    } */
}