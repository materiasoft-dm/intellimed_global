namespace IntelliMed.Core.Entities;

/// <summary>
/// A billable service/item in the practice's own catalog — a code, a description, and a standard
/// fee. Entirely user-managed; there is no external catalog or automatic rebate calculation here.
/// </summary>
public class BillingItem
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Fee { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
