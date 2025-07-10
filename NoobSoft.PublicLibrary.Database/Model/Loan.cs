namespace NoobSoft.PublicLibrary.Database.Model;

public class Loan
{
    public Guid BookId { get; set; }
    public Guid LoanerId { get; set; }
    
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    
    public decimal Fee { get; set; } 
    
    public bool IsReturned => ReturnDate.HasValue;
}