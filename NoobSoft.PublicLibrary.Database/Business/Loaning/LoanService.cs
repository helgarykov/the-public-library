using NoobSoft.PublicLibrary.Database.Model;
using NoobSoft.PublicLibrary.Database.Repository;

namespace NoobSoft.PublicLibrary.Database.Business.Loaning;

public class LoanService : ILoanService
{
    private readonly ILibraryRepository _repo;
    private readonly ITimeProvider _timeProvider;
    
    public LoanService(ILibraryRepository repo, ITimeProvider timeProvider)
    {
        _repo = repo;
        _timeProvider = timeProvider;
    }
    
    public bool TryLoanBook(Guid bookId, Guid loanerId)
    {
        var book = _repo.GetBookById(bookId);
        var loaner = _repo.GetLoanerById(loanerId);
        
        if(book == null || loaner == null)
            return false; 
        
        // Check if the book is already loaned
        var currentLoan = _repo.GetAllLoans().FirstOrDefault(l => l.BookId == bookId && l.ReturnedAt == null);
        if (currentLoan != null)
            return false;

        var now = _timeProvider.Now;
        var dueDate = now.AddDays(30);

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            LoanerId = loanerId,
            LoanedAt = now,
            DueAt = dueDate,
            ReturnedAt = null
        };
        
        _repo.AddLoan(loan);
        return true;
    }
}