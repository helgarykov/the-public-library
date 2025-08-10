namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public class InMemoryFeeLedgerRepository : IFeeLedgerRepository
{
    private readonly List<LedgerEntry> _entries = new();
    
    public void Append(LedgerEntry entry) => _entries.Add(entry);

    public IReadOnlyList<LedgerEntry> GetByLoaner(Guid loanerId) => 
        _entries.Where(e => e.LoanerId == loanerId)
            .OrderBy(e => e.PostedAt)
            .ToList();
    
}