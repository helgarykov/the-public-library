namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public record PaymentReceipt(
    Guid Id,
    Guid LoanerId,
    decimal Amount,
    DateTime PostedAt
    );