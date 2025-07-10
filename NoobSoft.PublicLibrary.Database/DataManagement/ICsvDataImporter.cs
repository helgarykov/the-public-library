using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.DataManagement;

public interface ICsvDataImporter
{
    (List<Author> Records, List<string> Errors) ImportAuthors();
    (List<Book> Records, List<string> Errors) ImportBooks();
    (List<Loaner> Records, List<string> Errors) ImportLoaners();
}