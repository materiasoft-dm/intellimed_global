namespace IntelliMed.Infrastructure.Services;

/// <summary>
/// Money rounding helper for invoice totals.
/// </summary>
public static class BillingMath
{
    /// <summary>Round a monetary amount to 2 dp using banker's rounding.</summary>
    public static decimal RoundMoney(decimal amount) =>
        Math.Round(amount, 2, MidpointRounding.ToEven);
}
