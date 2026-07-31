namespace IntelliMed.Core.Entities;

/// <summary>
/// A named price list (e.g. "Standard Rates", "Corporate Contract A") that can override the
/// catalog price of any <see cref="BillingItem"/> via <see cref="FeeScheduleItem"/>. Entirely
/// user-managed.
/// </summary>
public class FeeSchedule
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<FeeScheduleItem> Items { get; set; } = new List<FeeScheduleItem>();
}

/// <summary>A per-schedule price override for one billing item. Absence means the item's own base Fee applies.</summary>
public class FeeScheduleItem
{
    public int Id { get; set; }
    public int FeeScheduleId { get; set; }
    public int BillingItemId { get; set; }
    public decimal Fee { get; set; }

    public FeeSchedule? FeeSchedule { get; set; }
    public BillingItem? BillingItem { get; set; }
}
