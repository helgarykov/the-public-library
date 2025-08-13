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
        mock.SetupGet(p => p.LostFee).Returns(300m);
        mock.SetupGet(p => p.LostAfterDays).Returns(180);
        mock.SetupGet(p => p.SuspensionThreshold).Returns(100m);

        _policy = mock.Object;
        _svc = new FeeService(_policy, _fees); 
    }
}