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
        var loan = Loan(due: new DateTime(2025, 1, 10));
        var asOf = new DateTime(2025, 1, 9);

        Assert.Equal(0, _svc.OverdueDays(loan, asOf));
    }

    [Fact]
    public void OverudeDays_OnDue_Zero()
    {
        var loan = Loan(due: new DateTime(2025, 1, 10));
        var asOf = new DateTime(2025, 1, 10);

        Assert.Equal(0, _svc.OverdueDays(loan, asOf));
    }

    [Fact]
    public void OverdueDays_AfterDue_One()
    {
        var loan = Loan(due: new DateTime(2025, 1, 10));
        var asOf = new DateTime(2025, 1, 11);

        Assert.Equal(1, _svc.OverdueDays(loan, asOf));
    }

    // ---- AssessLoan ----
    [Fact]
    public void AssessLoan_NotOverdue_FlagsAndReason()
    {
        var loan = Loan(due: new DateTime(2025, 1, 10));
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
        var loan = Loan(due: new DateTime(2025, 1, 10));
        var asOf = new DateTime(2025, 1, 20);

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
        var loan = Loan(due: new DateTime(2025, 1, 10));
        var asOf = new DateTime(2025, 7, 10); // 181 days overdue => LostAfterDays=180 => lost

        var a = _svc.AssessLoan(loan, asOf);

        Assert.True(a.IsOverdue);
        Assert.True(a.IsLost);
        Assert.Equal("Lost fee", a.Reason);
        Assert.Equal(181, a.OverdueDays);
        Assert.Equal(300m, a.Fee); // Lost fee=300m
    }






    // ---- helper ----
    private static Loan Loan(DateTime due)
    {
        return new Loan
        {
            Id = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            LoanerId = Guid.NewGuid(),
            DueAt = due,
            LoanedAt = due.AddDays(-14),
            ReturnedAt = null
        };
    }
}   
    