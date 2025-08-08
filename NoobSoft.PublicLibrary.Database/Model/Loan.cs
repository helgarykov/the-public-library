namespace NoobSoft.PublicLibrary.Database.Model;

public class Loan
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid LoanerId { get; set; }
    
    public DateTime LoanedAt { get; set; }
    public DateTime DueAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    
    public decimal Fee { get; set; } 
    
    public bool IsReturned => ReturnedAt.HasValue;
}