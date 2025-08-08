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

    void AddLoan(Loan loan);
    // void UpdateLoan(Loan loan);
    
    List<Loan> GetAllLoans();

    List<Book> FindBooksByPartialIsbn(string partialIsbn);
    List<Person> FindPeopleByPartialNameOrBirthday(string search);

    List<string> ImportLog { get; }
    
    // Book? GetBookByIsbn(string isbn);
    // Person? GetPersonById(Guid id);


}