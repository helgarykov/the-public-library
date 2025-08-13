using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public interface IFeeService
{
    // Core calculations (pure; no I/O) 
    FeeAssessment AssessLoan(Loan loan, DateTime asOf);     // figures out late vs lost and amount
    bool IsLoanLost(Loan loan, DateTime asOf);              // >180 days overdue?
    int OverdueDays(Loan loan, DateTime asOf);              // max(0, floor(asOf - DueDate))
    
    // Account-level queries
    decimal GetOutstandingDebt(Guid loanerId);              // sum of unpaid ledger entries - payment
    bool IsSuspended(Guid loanerId);                        // debt >= 100?
    
    // Effects (mutate ledger/ repo)
    PostReturnResult PostReturn(Loan loan, DateTime returnedAt); // finalize fee for this book at return time
    LedgerEntry RecordPayment(Guid loanerId, decimal amount, DateTime when);
    IReadOnlyList<LedgerEntry> GetLedger(Guid loanerId);
}