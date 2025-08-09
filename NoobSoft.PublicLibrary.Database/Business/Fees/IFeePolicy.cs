namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public interface IFeePolicy
{
    int LostAfterDays { get; }  // 180
    decimal LostFee { get; }    // 300
    decimal InitialLateFee { get; } // 10
    int LateBlockSizeDays { get; }  // 5
    decimal LateBlockFee { get; }   // 5
    decimal SuspensionThreshold { get; }    // 100
}