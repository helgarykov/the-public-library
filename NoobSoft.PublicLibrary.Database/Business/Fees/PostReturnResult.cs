namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public record PostReturnResult(
    FeeAssessment Assessment,
    LedgerEntry? PostedEntry
    );