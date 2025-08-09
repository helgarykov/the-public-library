namespace NoobSoft.PublicLibrary.Database.Business.Fees;

public class FeePolicy : IFeePolicy
{
    public int LostAfterDays => 180;
    public decimal LostFee => 300m;
    public decimal InitialLateFee => 10m;
    public int LateBlockSizeDays => 5;
    public decimal LateBlockFee => 5m;
    public decimal SuspensionThreshold => 100m;
}