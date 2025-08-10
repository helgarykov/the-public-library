namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public interface IFeeLedgerRepository
{
    void Append(LedgerEntry entry);     // fees and payments → one append-only stream

    IReadOnlyList<LedgerEntry> GetByLoaner(Guid loanerId);
    
    decimal GetOutstandingDebt(Guid loanerId);
}