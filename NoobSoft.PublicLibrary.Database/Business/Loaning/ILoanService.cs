using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.Business.Loaning;

public interface ILoanService
{
    bool TryLoanBook(Guid bookId, Guid loanerId);
    //bool ReturnBook(Guid bookId);
    //IEnumerable<Book> GetBooksLoanedByPerson(Guid loanerId);
    //IEnumerable<Loan> GetLoansForBook(Guid bookId);
    //Loan? IsBookLoaned(Guid bookId);
}