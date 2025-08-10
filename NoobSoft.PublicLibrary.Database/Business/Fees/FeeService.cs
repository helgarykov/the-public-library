using NoobSoft.PublicLibrary.Database.Business.Fees.Internal;
using NoobSoft.PublicLibrary.Database.Model;
using NoobSoft.PublicLibrary.Database.Repository;

namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public class FeeService : IFeeService
{
    private readonly IFeePolicy _policy;
    private readonly IFeeLedgerRepository _fees;
    private readonly ILibraryRepository _lib;
    
    public FeeService(IFeePolicy policy, IFeeLedgerRepository fees, ILibraryRepository lib)
    {
        _policy = policy;
        _fees = fees;
        _lib = lib;
    }
    public FeeAssessment AssessLoan(Loan loan, DateTime asOf)
    {
        int overdueDays = OverdueDays(loan, asOf);
        decimal fee = FeeCalculator.CalculateFee(overdueDays, _policy);
        
        bool isLost = FeeCalculator.IsLost(overdueDays, _policy);
        bool isOverdue = overdueDays > 0;
        
        return new FeeAssessment(
            IsOverdue: isOverdue,
            OverdueDays: overdueDays,
            IsLost: isLost,
            Fee: fee,
            Reason: !isOverdue ? "None" :
                isLost ? "Lost fee" : "Late fee"
        );
    }

    public bool IsLoanLost(Loan loan, DateTime asOf)
    {
        throw new NotImplementedException();
    }

    public int OverdueDays(Loan loan, DateTime asOf) =>
        Math.Max(0, (asOf.Date - loan.DueAt.Date).Days);

    public decimal GetOutstandingDebt(Guid loanerId)
    {
        throw new NotImplementedException();
    }

    public bool IsSuspended(Guid loanerId)
    {
        throw new NotImplementedException();
    }

    public LedgerEntry PostReturn(Loan loan, DateTime returnedAt)
    {
        throw new NotImplementedException();
    }

    public PaymentReceipt RecordPayment(Guid loanerId, decimal amount, DateTime when)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<LedgerEntry> GetLedger(Guid loanerId)
    {
        throw new NotImplementedException();
    }
}