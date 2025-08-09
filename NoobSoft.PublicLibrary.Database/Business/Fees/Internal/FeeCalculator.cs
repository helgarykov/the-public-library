namespace NoobSoft.PublicLibrary.Database.Business.Fees.Internal;
/// <summary>
/// Internal pure helpers
/// </summary>
internal static class FeeCalculator
{
    // Late fee: 10 coins once when overdue (days >= 1), plus 5 coins for every additional FULL 5-day block.
    // Lost fee: if overdue > 180 days → flat 300 coins, replaces any late fee.
    public static decimal CalculateFee(int overdueDays, IFeePolicy policy)
    {
        if (overdueDays <= 0)
            return 0m;
        
        // Lost book case
        if (overdueDays > policy.LostAfterDays)
            return policy.LostFee;
        
        // Base late fee
        decimal fee = policy.InitialLateFee;
        
        // Additional late fee per block
        var extraDays = Math.Max(0, overdueDays - policy.LateBlockSizeDays);
        var extraBlocks = extraDays / policy.LateBlockSizeDays;     // full blocks only
        fee += policy.LateBlockFee * extraBlocks;

        return fee;
    }
    
    public static bool IsLost(int overdueDays, IFeePolicy policy) =>
        overdueDays > policy.LostAfterDays;
    
    public static bool IsSuspended(decimal outstandingDebt, IFeePolicy policy) =>
        outstandingDebt >= policy.SuspensionThreshold;
    
}