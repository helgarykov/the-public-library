using System.Collections;
using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.Business;

public interface ILibraryService
{
    // Loaning 
    bool TryLoanBook(Guid bookId, Guid loanerId);
    
    // Returns
    bool ReturnBook(Guid bookId);
    
    // Fees
    decimal CalculateFeeForLoaner(Guid loanerId);
    
    // Status
    IEnumerable<Book> GetBooksLoanedByPerson(Guid loanerId);
    IEnumerable<Loan> GetLoansForBook(Guid bookId);

    // Reservations
    bool ReserveBook(Guid bookId, Guid loanerId);
    IEnumerable<Guid> GetReservationsForBook(Guid bookId);

    // Popularity
    IEnumerable<Book> GetMostPopularBooks(int top = 5);

    // Setup
    void LoadData(); // Delegates to ILibraryRepository.LoadData
}