namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public record LedgerEntry(
    Guid Id,
    Guid LoanerId,
    Guid? LoanId,
    decimal Amount,
    string Reason,
    DateTime PostedAt
    );