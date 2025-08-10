using NoobSoft.PublicLibrary.Database.Model;

namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public class FeeService : IFeeService
{
    public FeeAssessment AssessLoan(Loan loan, DateTime asOf)
    {
        throw new NotImplementedException();
    }

    public bool IsLoanLost(Loan loan, DateTime asOf)
    {
        throw new NotImplementedException();
    }

    public int OverdueDays(Loan loan, DateTime asOf)
    {
        throw new NotImplementedException();
    }

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