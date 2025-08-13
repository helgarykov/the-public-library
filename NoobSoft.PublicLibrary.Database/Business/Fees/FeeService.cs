using System.Runtime.Intrinsics.X86;
using NoobSoft.PublicLibrary.Database.Business.Fees.Internal;
using NoobSoft.PublicLibrary.Database.Model;
using NoobSoft.PublicLibrary.Database.Repository;

namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public class FeeService : IFeeService
{
    private readonly IFeePolicy _policy;
    private readonly IFeeLedgerRepository _fees;
    
    public FeeService(IFeePolicy policy, IFeeLedgerRepository fees)
    {
        _policy = policy;
        _fees = fees;
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
        var overdue = OverdueDays(loan, asOf);
        return FeeCalculator.IsLost(overdue, _policy);
    }

    public int OverdueDays(Loan loan, DateTime asOf) =>
        Math.Max(0, (asOf.Date - loan.DueAt.Date).Days);

    public decimal GetOutstandingDebt(Guid loanerId) =>
        _fees.GetOutstandingDebt(loanerId);

    public bool IsSuspended(Guid loanerId) =>
        _fees.GetOutstandingDebt(loanerId) >= _policy.SuspensionThreshold;

    public PostReturnResult PostReturn(Loan loan, DateTime returnedAt)
    {
       var assessment = AssessLoan(loan, returnedAt);
       if(!assessment.IsOverdue)
           return new PostReturnResult(assessment, null);
       
       var entry = new LedgerEntry(
           Id: Guid.NewGuid(),
           LoanerId: loan.LoanerId,
           LoanId: loan.Id,
           Amount: assessment.Fee,
           Reason: assessment.Reason,
           PostedAt: returnedAt
       );
       
       _fees.Append(entry);
       return new PostReturnResult(assessment, entry);
    }

    public LedgerEntry RecordPayment(Guid loanerId, decimal amount, DateTime when)
    {
        if (amount <= 0)
            throw new ArgumentException(nameof(amount), "Payment amount must be positive.");

        // Payments are stored as negative amounts in the ledger
        var entry = new LedgerEntry(
            Id: Guid.NewGuid(),
            LoanerId: loanerId,
            LoanId: null,
            Amount: -amount,
            Reason: "Payment received",
            PostedAt: when
        );

        _fees.Append(entry);
        return entry;
    }

    public IReadOnlyList<LedgerEntry> GetLedger(Guid loanerId)
    {
        return _fees.GetByLoaner(loanerId)
            .OrderByDescending(e => e.PostedAt)
            .ThenByDescending(e => e.Id) // stable ordering on ties
            .ToList().AsReadOnly();
    }
}