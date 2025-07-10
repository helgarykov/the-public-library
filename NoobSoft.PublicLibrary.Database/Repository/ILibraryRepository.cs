using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.Repository;

public interface ILibraryRepository
{
    void LoadData();
    List<Author> GetAllAuthors();
    List<Book> GetAllBooks();
    List<Loaner> GetAllLoaners();

    Author? GetAuthorById(Guid id);
    Book? GetBookById(Guid id);
    Loaner? GetLoanerById(Guid id);

    List<Book> FindBooksByPartialIsbn(string partialIsbn);
    List<Person> FindPeopleByPartialNameOrBirthday(string search);

    List<string> ImportLog { get; }
    
    // Book? GetBookByIsbn(string isbn);
    // Person? GetPersonById(Guid id);


}