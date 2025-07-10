using NoobSoft.PublicLibrary.Database.Model;
using NoobSoft.PublicLibrary.Database.Repository;

namespace NoobSoft.PublicLibrary.Database.Business;

/// <summary>
/// Provides the core business logic for the public library system.
/// Encapsulates operations such as book loaning, returning, fee calculation,
/// reservation management, and popularity ranking of books.
/// </summary>
public class LibraryService : ILibraryService
{
    private readonly ILibraryRepository _repository;
    private readonly ITimeProvider _timeProvider;

    public LibraryService(ILibraryRepository repo, ITimeProvider timeProvider)
    {
        _repository = repo;
        _timeProvider = timeProvider;
    }
    public bool TryLoanBook(Guid bookId, Guid loanerId)
    {
        throw new NotImplementedException();
    }

    public bool ReturnBook(Guid bookId)
    {
        throw new NotImplementedException();
    }

    public decimal CalculateFeeForLoaner(Guid loanerId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Book> GetBooksLoanedByPerson(Guid loanerId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Loan> GetLoansForBook(Guid bookId)
    {
        throw new NotImplementedException();
    }

    public bool ReserveBook(Guid bookId, Guid loanerId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Guid> GetReservationsForBook(Guid bookId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Book> GetMostPopularBooks(int top = 5)
    {
        throw new NotImplementedException();
    }

    public void LoadData()
    {
        _repository.LoadData();
    }
}