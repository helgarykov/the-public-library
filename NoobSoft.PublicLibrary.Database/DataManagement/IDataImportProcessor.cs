using CsvHelper;

namespace NoobSoft.PublicLibrary.Database.DataManagement;

/// <summary>
/// Interface for implementing the import of data from csv. files.
/// Is used to access the author/loaner/book data through the ID number.
/// </summary>
public interface IDataImportProcessor
{
    /// <summary>
    /// Reads data from a csv. file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filename">the file to read</param>
    /// <returns> an array of strings as lines </returns>
    string[] GetIdRecords(CsvReader filename);
    
    void StreamReader GetData(path);

    /// <summary>
    /// Loads data from a file as a string [] and saves it as Dictionary type.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Dictionary<string, List<object>> ImportData(string file);

}