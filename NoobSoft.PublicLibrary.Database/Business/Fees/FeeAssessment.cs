namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public record FeeAssessment(
    bool IsOverdue,
    int OverdueDays,
    bool IsLost,               // OverdueDays > 180
    decimal Fee,               // amount for this loan as of 'asOf'
    string Reason              // "Late fee", "Lost fee", or "None"
);