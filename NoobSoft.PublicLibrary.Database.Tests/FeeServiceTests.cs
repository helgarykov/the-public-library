using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using NoobSoft.PublicLibrary.Database.Business.Fees;
using NoobSoft.PublicLibrary.Database.Business.Fees.Internal;
using NoobSoft.PublicLibrary.Database.Model;
using NoobSoft.PublicLibrary.Database.Repository;

namespace NoobSoft.PublicLibrary.Database.Tests;

public class FeeServiceTests
{
    private readonly InMemoryFeeLedgerRepository _fees = new();
    private readonly IFeePolicy _policy;
    private readonly FeeService _svc;

    public FeeServiceTests()
    {
        /// <summary>
        /// Using a mock IFeePolicy here ensures the tests are isolated from production defaults.
        /// If FeeCalculator relied on FeePolicy's built-in values, tests could break whenever 
        /// those defaults change, even if FeeService's logic is still correct. By explicitly 
        /// setting only the needed properties via Moq, we keep the tests deterministic, 
        /// focused, and resistant to unrelated changes in FeePolicy.
        /// </summary>
        var mock = new Mock<IFeePolicy>();
        mock.SetupGet(p => p.InitialLateFee).Returns(10m);
        mock.SetupGet(p => p.LateBlockFee).Returns(5m);
        mock.SetupGet(p => p.LateBlockSizeDays).Returns(5);
        mock.SetupGet(p => p.LostFee).Returns(300m);
        mock.SetupGet(p => p.LostAfterDays).Returns(180);
        mock.SetupGet(p => p.SuspensionThreshold).Returns(100m);

        _policy = mock.Object;
        _svc = new FeeService(_policy, _fees);
    }

    // ---- OverdueDays ----
    [Fact]
    public void OverdueDays_BeforeDue_Zero()
    {
        var loan = MakeLoan(new DateTime(2025, 1, 10));
        var asOf = new DateTime(2025, 1, 9);

        Assert.Equal(0, _svc.OverdueDays(loan, asOf));
    }

    [Fact]
    public void OverudeDays_OnDue_Zero()
    {
        var loan = MakeLoan(new DateTime(2025, 1, 10));
        var asOf = new DateTime(2025, 1, 10);

        Assert.Equal(0, _svc.OverdueDays(loan, asOf));
    }

    [Fact]
    public void OverdueDays_AfterDue_One()
    {
        var loan = MakeLoan(
            loanedAt: new DateTime(2024, 12, 11)); // +30 = 2025-01-10 due
        
        var asOf = new DateTime(2025, 1, 11);   // 2025-01-10 + 1 = 2025-01-11

        Assert.Equal(1, _svc.OverdueDays(loan, asOf));
    }

    // ---- AssessLoan ----
    [Fact]
    public void AssessLoan_NotOverdue_FlagsAndReason()
    {
        var loan = MakeLoan(new DateTime(2025, 1, 10));
        var asOf = new DateTime(2025, 1, 10);

        var a = _svc.AssessLoan(loan, asOf);

        Assert.False(a.IsOverdue);
        Assert.Equal("None", a.Reason);
        Assert.Equal(0, a.OverdueDays);
        Assert.Equal(0m, a.Fee);
    }

    [Fact]
    public void AssessLoan_OverdueButNotLost_FlagsAndReason()
    {
        var loan = MakeLoan(
            loanedAt: new DateTime(2024, 12, 11)); // +30 = 2025-01-10 due

        var asOf = new DateTime(2025, 1, 20);   // 10 days overdue

        var a = _svc.AssessLoan(loan, asOf);

        Assert.True(a.IsOverdue);
        Assert.False(a.IsLost);
        Assert.Equal("Late fee", a.Reason);
        Assert.Equal(10, a.OverdueDays);
        Assert.Equal(15m, a.Fee); // 10 + 5 late block fee
    }

    [Fact]
    public void AssessLoan_Lost_FlagsAndReason()
    {
        var loan = MakeLoan(
            loanedAt: new DateTime(2024, 12, 11)); // +30 = 2025-01-10 due // 2025-01-10 + 180 = 2025-07-10
        
        var asOf = new DateTime(2025, 7, 10); // 181 days overdue => LostAfterDays=180 => lost

        var a = _svc.AssessLoan(loan, asOf);

        Assert.True(a.IsOverdue);
        Assert.True(a.IsLost);
        Assert.Equal("Lost fee", a.Reason);
        Assert.Equal(181, a.OverdueDays);
        Assert.Equal(300m, a.Fee); // Lost fee=300m
    }
     
    // ---- PostReturn ----
    [Fact]
    public void PostReturn_NotOverdue_NoLedgerEntry()
    {
        var loan = MakeLoan(new DateTime(2025, 1, 10));
        var returned = new DateTime(2025, 1, 10);

        var result = _svc.PostReturn(loan, returned);
        
        Assert.False(result.Assessment.IsOverdue);
        Assert.Null(result.PostedEntry);
        Assert.Empty(_fees.GetByLoaner(loan.LoanerId));
    }

    [Fact]
    public void PostReturn_Overdue_AppendsLedgerEntry()
    {
        var loan = MakeLoan(
            loanedAt: new DateTime(2024, 12, 11)); // +30 = 2025-01-10 due
       
        var returned = new DateTime(2025, 1, 20);  // 2025-01-10 + 10 = 2025-01-20 => 10 days overdue
        
        var before = _fees.GetByLoaner(loan.LoanerId).Count;
        var result = _svc.PostReturn(loan, returned);
        var afterList = _fees.GetByLoaner(loan.LoanerId);
        
        Assert.True(result.Assessment.IsOverdue);
        Assert.NotNull(result.PostedEntry);
        Assert.Equal(before + 1, afterList.Count);

        var entry = afterList.Last();
        Assert.Equal(loan.LoanerId, entry.LoanerId);
        Assert.Equal(loan.Id, entry.LoanId);
        Assert.Equal(returned, entry.PostedAt);
        Assert.Equal(result.Assessment.Reason, entry.Reason);
        Assert.Equal(result.Assessment.Fee, entry.Amount);
    }
    
    // ----RecordPayment ----
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void RecordPayment_AmountLessOrEqualZero_ThrowsArgumentException(decimal invalidAmount)
    {
        var loanerId = Guid.NewGuid();
        
        var ex = Assert.Throws<ArgumentException>(() => 
            _svc.RecordPayment(loanerId, invalidAmount, DateTime.UtcNow));
        
        Assert.Equal("Payment amount must be positive.", ex.ParamName);
    }

    [Fact]
    public void RecordPayment_Positive_AppendsNegativeAmount()
    {
        var loanerId = Guid.NewGuid();
        var when = new DateTime(2025, 1, 15);
        
        var entry = _svc.RecordPayment(loanerId, 50m, when);
        
        Assert.Equal(loanerId, entry.LoanerId);
        Assert.Null(entry.LoanId);
        Assert.Equal(when, entry.PostedAt);
        Assert.Equal("Payment received", entry.Reason);
        Assert.Equal(-50m, entry.Amount); 
        
        var repoEntry = _fees.GetByLoaner(loanerId).Single();
        Assert.Equal(entry.Id, repoEntry.Id);
    }
    
    // ---- GetOutstandingDebt & IsSuspended ----
    [Fact]
    public void GetOutstandingDebt_SumsFeesAndPayments()
    {
        var loanerId = Guid.NewGuid();
        
        _fees.Append(new LedgerEntry( Guid.NewGuid(), loanerId, Guid.NewGuid(), 40m, "Late fee", new DateTime(2025,1,10)));
        _fees.Append(new LedgerEntry(Guid.NewGuid(), loanerId, Guid.NewGuid(), 300m, "Lost fee", new DateTime(2025,1,20)));
        _fees.Append(new LedgerEntry( Guid.NewGuid(), loanerId, null, -50m, "Payment received", new DateTime(2025,1,15)));

        var debt = _svc.GetOutstandingDebt(loanerId);
        Assert.Equal(290m, debt); // 40 + 300 - 50
    }

    [Fact]
    public void IsSuspended_usesThreshhold()
    {
        var loanerId = Guid.NewGuid();
        
        // < threshold = 100m
        _fees.Append(new LedgerEntry(Guid.NewGuid(), loanerId,Guid.NewGuid(), 90m, "Late fee", new DateTime(2025,1,10)));
        Assert.False(_svc.IsSuspended(loanerId));
        
        // == threshold 
        _fees.Append(new LedgerEntry(Guid.NewGuid(), loanerId, Guid.NewGuid(), 10m, "Late fee", new DateTime(2025,1,11)));
        Assert.True(_svc.IsSuspended(loanerId));
        
        // < threshold again
        _fees.Append(new LedgerEntry(Guid.NewGuid(), loanerId, null, -5m, "Payment received", new DateTime(2025,1,12)));   
        Assert.False(_svc.IsSuspended(loanerId));
    }
    
    // ---- GetLedger ordering ----
    [Fact]
    public void GetLedger_OrdersByDateDescThenIdDesc()
    {
        var loanerId = Guid.NewGuid();
        var t = new DateTime(2025, 9, 23);
        
        var e1 = new LedgerEntry(new Guid(), loanerId, Guid.NewGuid(), 10m, "Late", t);
        var e2 = new LedgerEntry(new Guid(), loanerId, Guid.NewGuid(), 20m, "Late", t.AddDays(1));
        var e3_sameTime = new LedgerEntry(new Guid(), loanerId, Guid.NewGuid(), 30m, "Late", t.AddDays(1));
        
        _fees.Append(e1);
        _fees.Append(e2);
        _fees.Append(e3_sameTime);
        
        var ordered = _svc.GetLedger(loanerId).ToList();
        
        // Newest date first
        Assert.Equal(new[] {t.AddDays(1), t.AddDays(1), t }, ordered.Select(x => x.PostedAt));
        
        // For items with the same timestamp, higher Guid (lexicographically) should come first due to ThenByDescending(Id)
        
        var expectedIds = ordered
            .OrderByDescending(x => x.PostedAt)
            .ThenByDescending(x => x.Id)
            .Select(x => x.Id)
            .ToList();
        
        var actualIds = ordered.Select(x => x.Id).ToList();
        Assert.Equal(expectedIds, actualIds);
    }

    // ---- helper ----
    private static Loan MakeLoan(
        DateTime loanedAt,
        int loanPeriodDays = 30,
        Guid? loanerId = null,
        Guid? bookId = null,
        DateTime? dueAt = null,
        DateTime? returnedAt = null,
        decimal fee = 0m)
    {
        return new Loan
        {
            Id = Guid.NewGuid(),
            BookId = bookId ?? Guid.NewGuid(),
            LoanerId = loanerId ?? Guid.NewGuid(),
            LoanedAt = loanedAt,
            DueAt = dueAt ?? loanedAt.AddDays(loanPeriodDays),
            ReturnedAt = returnedAt,
            Fee = fee
        };
    }

}   
    