using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.DataManagement;

public class DataImportFromCsv : IDataImportProcessor
{
    private Guid Id { get; }
    private string Name { get; }
    private DateTime DateTime { get; }

    protected DataImportFromCsv()
    {
        Id = Guid.Empty;
        Name = string.Empty;
        DateTime = new DateTime();
    }

    public StreamReader GetData(string filename)
    {
        var fileContentLines = File.ReadAllLines(Path.Combine("Data", filename));
        return fileContentLines;
    }
    

    public Dictionary<string, List<object>> ImportData(string file)
    {
        Dictionary<string, List<object>> results = new();

        var fileContents = GetData(file);

        foreach (var line in fileContents)
        {
            
        }

    }
}